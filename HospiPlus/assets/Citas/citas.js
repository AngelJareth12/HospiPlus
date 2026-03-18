var calendar = document.getElementById("calendar-table");
var gridTable = document.getElementById("table-body");
var currentDate = new Date();
var selectedDate = currentDate;
var selectedDayBlock = null;
var globalEventObj = {};

// responsive blur
const updateBlur = () => {
    gb.setAttribute('stdDeviation', Math.min(window.innerWidth / 1600 * 10, 10));
};
updateBlur();
window.addEventListener('resize', updateBlur);





var sidebar = document.getElementById("sidebar");
function createCalendar(date, side) {
    var currentDate = date;
    var startDate = new Date(currentDate.getFullYear(), currentDate.getMonth(), 1);

    var monthTitle = document.getElementById("month-name");
    var monthName = currentDate.toLocaleString("en-US", { month: "long" });
    var yearNum = currentDate.toLocaleString("en-US", { year: "numeric" });
    monthTitle.innerHTML = `${monthName} ${yearNum}`;

    if (side == "left") {
        gridTable.className = "animated fadeOutRight";
    } else {
        gridTable.className = "animated fadeOutLeft";
    }

    setTimeout(() => {
        gridTable.innerHTML = "";

        var newTr = document.createElement("div");
        newTr.className = "row";
        var currentTr = gridTable.appendChild(newTr);

        var lastDay = new Date(currentDate.getFullYear(), currentDate.getMonth() + 1, 0).getDate();

        for (let i = 1; i <= lastDay; i++) {
            let tempDate = new Date(currentDate.getFullYear(), currentDate.getMonth(), i);
            let dayOfWeek = tempDate.getDay();

            // Saltar sábados (6) y domingos (0)
            if (dayOfWeek === 0 || dayOfWeek === 6) continue;

            if (currentTr.children.length >= 5) {
                currentTr = gridTable.appendChild(addNewRow());
            }

            let currentDay = document.createElement("div");
            currentDay.className = "col";

            if (
                (selectedDayBlock == null && i == currentDate.getDate()) ||
                selectedDate.toDateString() == tempDate.toDateString()
            ) {
                selectedDate = tempDate;

                document.getElementById("eventDayName").innerHTML = selectedDate.toLocaleString("en-US", {
                    month: "long",
                    day: "numeric",
                    year: "numeric"
                });

                selectedDayBlock = currentDay;
                setTimeout(() => {
                    currentDay.style.backgroundColor = "#FFFFED";
                    currentDay.style.color = "black";                
                }, 900);
            }

            currentDay.innerHTML = i;

            // Mostrar marcas si existen eventos
            if (globalEventObj[tempDate.toDateString()]) {
                let eventMark = document.createElement("div");
                eventMark.className = "day-mark";
                currentDay.appendChild(eventMark);
            }

            currentTr.appendChild(currentDay);
        }

        // Completar la última fila con espacios vacíos si faltan columnas
        for (let i = currentTr.children.length; i < 5; i++) {
            let emptyDivCol = document.createElement("div");
            emptyDivCol.className = "col empty-day";
            currentTr.appendChild(emptyDivCol);
        }

        if (side == "left") {
            gridTable.className = "animated fadeInLeft";
        } else {
            gridTable.className = "animated fadeInRight";
        }

        function addNewRow() {
            let node = document.createElement("div");
            node.className = "row";
            return node;
        }

    }, !side ? 0 : 270);
}


createCalendar(currentDate);

var todayDayName = document.getElementById("todayDayName");
todayDayName.innerHTML = "Today is " + currentDate.toLocaleString("en-US", {
    weekday: "long",
    day: "numeric",
    month: "short"
});

var prevButton = document.getElementById("prev");
var nextButton = document.getElementById("next");

prevButton.onclick = function changeMonthPrev() {
    currentDate = new Date(currentDate.getFullYear(), currentDate.getMonth() - 1);
    createCalendar(currentDate, "left");
}
nextButton.onclick = function changeMonthNext() {
    currentDate = new Date(currentDate.getFullYear(), currentDate.getMonth() + 1);
    createCalendar(currentDate, "right");
}

function addEvent(title, desc) {
    if (!globalEventObj[selectedDate.toDateString()]) {
        globalEventObj[selectedDate.toDateString()] = {};
    }
    globalEventObj[selectedDate.toDateString()][title] = desc;
}

function showEvents() {
    let sidebarEvents = document.getElementById("sidebarEvents");
    let objWithDate = globalEventObj[selectedDate.toDateString()];

    sidebarEvents.innerHTML = "";

    if (objWithDate) {
        let eventsCount = 0;
        for (key in globalEventObj[selectedDate.toDateString()]) {
            let eventContainer = document.createElement("div");
            eventContainer.className = "eventCard";

            let eventHeader = document.createElement("div");
            eventHeader.className = "eventCard-header";

            let eventDescription = document.createElement("div");
            eventDescription.className = "eventCard-description";

            eventHeader.appendChild(document.createTextNode(key));
            eventContainer.appendChild(eventHeader);

            eventDescription.appendChild(document.createTextNode(objWithDate[key]));
            eventContainer.appendChild(eventDescription);

            let markWrapper = document.createElement("div");
            markWrapper.className = "eventCard-mark-wrapper";
            let mark = document.createElement("div");
            mark.classList = "eventCard-mark";
            markWrapper.appendChild(mark);
            eventContainer.appendChild(markWrapper);

            sidebarEvents.appendChild(eventContainer);

            eventsCount++;
        }
        let emptyFormMessage = document.getElementById("emptyFormTitle");
        emptyFormMessage.innerHTML = `${eventsCount} events now`;
    } else {
        let emptyMessage = document.createElement("div");
        emptyMessage.className = "empty-message";
        emptyMessage.innerHTML = "Sorry, no events to selected date";
        sidebarEvents.appendChild(emptyMessage);
        let emptyFormMessage = document.getElementById("emptyFormTitle");
        emptyFormMessage.innerHTML = "No events now";
    }
}
var formattedDate, formattedToday, fechaConfirmacion,fechaFinal;
gridTable.onclick = function (e) {

    if (!e.target.classList.contains("col") || e.target.classList.contains("empty-day")) {
        return;
    }

    if (selectedDayBlock) {
        if (selectedDayBlock.classList.contains("blue") && selectedDayBlock.classList.contains("lighten-3")) {
            selectedDayBlock.classList.remove("blue");
            selectedDayBlock.classList.remove("lighten-3");
        }
    }
    selectedDayBlock = e.target;
    selectedDayBlock.classList.add("blue");
    selectedDayBlock.classList.add("lighten-3");

    selectedDate = new Date(currentDate.getFullYear(), currentDate.getMonth(), parseInt(e.target.innerHTML));
    const year = selectedDate.getFullYear();
    const month = String(selectedDate.getMonth() + 1).padStart(2, '0'); // Month is 0-indexed, so add 1
    const day = String(selectedDate.getDate()).padStart(2, '0');

     formattedDate = `${year}${month}${day}`;

    const today = new Date(); // Gets today's date

    // Create a new Date object for tomorrow
    const tomorrow = new Date(today);
    tomorrow.setDate(today.getDate() + 1); // Add 1 day to today's date

    const tomorrowYear = tomorrow.getFullYear();
    const tomorrowMonth = String(tomorrow.getMonth() + 1).padStart(2, '0');
    const tomorrowDay = String(tomorrow.getDate()).padStart(2, '0');
    formattedToday = `${tomorrowYear}${tomorrowMonth}${tomorrowDay}`;

    fechaFinal = `${year}/${month}/${day}`;

    showEvents();

    document.getElementById("eventDayName").innerHTML = selectedDate.toLocaleString("en-US", {
        month: "long",
        day: "numeric",
        year: "numeric"
    });

    fechaConfirmacion = selectedDate.toLocaleString("en-US", {
        month: "long",
        day: "numeric",
        year: "numeric"
    });
}

var changeFormButton = document.getElementById("changeFormButton");
var addForm = document.getElementById("addForm")
changeFormButton.onclick = function () {

    if (formattedDate < formattedToday) {
        Swal.fire({
            icon: 'error',
            title: 'Fecha Inválida',
            text: 'La fecha seleccionada no puede ser anterior a la fecha actual. Por favor, elija una fecha válida.',
            confirmButtonText: 'Aceptar'
        });
    } else if (formattedDate === formattedToday) {
        Swal.fire({
            icon: 'warning',
            title: 'Selección de Fecha Requerida',
            text: 'La acción solicitada requiere que la fecha seleccionada sea al menos después dos días de la creación de la cita. Por favor, ajuste su selección.',
            confirmButtonText: 'Entendido'
        });
    } else {
        fechaConfirmacion = selectedDate.toLocaleString("es-ES", {
            month: "long",
            day: "numeric",
            year: "numeric"
        });
        $("#fechaCita").text(fechaConfirmacion);
        console.log(fechaFinal)
        $(".main-wrapper").css({ "display": 'none' });
        $(".contenedorReloj").css({ "display": 'block' });
        $("#btnContinuar").css({ "display": 'block' });
    }


}

var btnRCalendario = document.getElementById("regresarCalendario");

var cancelAdd = document.getElementById("cancelAdd");
cancelAdd.onclick = function (e) {
    addForm.style.top = "100%";
    let inputs = addForm.getElementsByTagName("input");
    for (let i = 0; i < inputs.length; i++) {
        inputs[i].value = "";
    }
    let labels = addForm.getElementsByTagName("label");
    for (let i = 0; i < labels.length; i++) {
        labels[i].className = "";
    }
}

var addEventButton = document.getElementById("addEventButton");
addEventButton.onclick = function (e) {
    let title = document.getElementById("eventTitleInput").value.trim();
    let desc = document.getElementById("eventDescInput").value.trim();

    if (!title || !desc) {
        document.getElementById("eventTitleInput").value = "";
        document.getElementById("eventDescInput").value = "";
        let labels = addForm.getElementsByTagName("label");
        for (let i = 0; i < labels.length; i++) {
            labels[i].className = "";
        }
        return;
    }

    addEvent(title, desc);
    showEvents();

    if (!selectedDayBlock.querySelector(".day-mark")) {
        selectedDayBlock.appendChild(document.createElement("div")).className = "day-mark";
    }

    let inputs = addForm.getElementsByTagName("input");
    for (let i = 0; i < inputs.length; i++) {
        inputs[i].value = "";
    }
    let labels = addForm.getElementsByTagName("label");
    for (let i = 0; i < labels.length; i++) {
        labels[i].className = "";
    }
}

console.log(selectedDate.toLocaleString("en-US", {
    month: "long",
    day: "numeric",
    year: "numeric"
}))

console.log(TIME);
