'use strict';

//Opening or closing side bar

const elementToggleFunc = function (elem) {
    elem.classList.toggle("active");
    if (elem == sidebar && sidebar.classList.contains("active")) {
        $('#content').css({
            'display': 'none'
        });
    } else {
        $('#content').css({
            'display': 'block'
        });
    }
}

const sidebar = document.querySelector("[data-sidebar]");
const sidebarBtn = document.querySelector("[data-sidebar-btn]");

sidebarBtn.addEventListener("click", function () {
    elementToggleFunc(sidebar);
})

//Activating Modal-testimonial

const testimonialsItem = document.querySelectorAll('[data-testimonials-item]');
const modalContainer = document.querySelector('[data-modal-container]');
const modalCloseBtn = document.querySelector('[data-modal-close-btn]');
const overlay = document.querySelector('[data-overlay]');

const modalImg = document.querySelector('[data-modal-img]');
const modalTitle = document.querySelector('[data-modal-title]');
const modalText = document.querySelector('[data-modal-text]');

const testimonialsModalFunc = function () {
    modalContainer.classList.toggle('active');
    overlay.classList.toggle('active');
}

for (let i = 0; i < testimonialsItem.length; i++) {
    testimonialsItem[i].addEventListener('click', function () {
        modalImg.src = this.querySelector('[data-testimonials-avatar]').src;
        modalImg.alt = this.querySelector('[data-testimonials-avatar]').alt;
        modalTitle.innerHTML = this.querySelector('[data-testimonials-title]').innerHTML;
        modalText.innerHTML = this.querySelector('[data-testimonials-text]').innerHTML;

        testimonialsModalFunc();
    })
}

//Activating close button in modal-testimonial

modalCloseBtn.addEventListener('click', testimonialsModalFunc);
overlay.addEventListener('click', testimonialsModalFunc);
/*
//Activating Filter Select and filtering options

const select = document.querySelector('[data-select]');
const selectItems = document.querySelectorAll('[data-select-item]');
const selectValue = document.querySelector('[data-select-value]');
const filterBtn = document.querySelectorAll('[data-filter-btn]');

select.addEventListener('click', function () { elementToggleFunc(this); });

for (let i = 0; i < selectItems.length; i++) {
    selectItems[i].addEventListener('click', function () {

        let selectedValue = this.innerText.toLowerCase();
        selectValue.innerText = this.innerText;
        elementToggleFunc(select);
        filterFunc(selectedValue);

    });
}
*/

/*const filterItems = document.querySelectorAll('[data-filter-item]');

const filterFunc = function (selectedValue) {
    for (let i = 0; i < filterItems.length; i++) {
        if (selectedValue == "all") {
            filterItems[i].classList.add('active');
        } else if (selectedValue == filterItems[i].dataset.category) {
            filterItems[i].classList.add('active');
        } else {
            filterItems[i].classList.remove('active');
        }
    }
}

//Enabling filter button for larger screens 

let lastClickedBtn = filterBtn[0];

for (let i = 0; i < filterBtn.length; i++) {

    filterBtn[i].addEventListener('click', function () {

        let selectedValue = this.innerText.toLowerCase();
        selectValue.innerText = this.innerText;
        filterFunc(selectedValue);

        lastClickedBtn.classList.remove('active');
        this.classList.add('active');
        lastClickedBtn = this;

    })
}
*/
// Enabling Contact Form

const form = document.querySelector('[data-form]');
const formInputs = document.querySelectorAll('[data-form-input]');
const formBtn = document.querySelector('[data-form-btn]');

for (let i = 0; i < formInputs.length; i++) {
    formInputs[i].addEventListener('input', function () {
        if (form.checkValidity()) {
            formBtn.removeAttribute('disabled');
        } else {
            formBtn.setAttribute('disabled', '');
        }
    })
}

// Enabling Page Navigation 

const navigationLinks = document.querySelectorAll('[data-nav-link]');
const pages = document.querySelectorAll('[data-page]');

for (let i = 0; i < navigationLinks.length; i++) {
    navigationLinks[i].addEventListener('click', function () {

        for (let i = 0; i < pages.length; i++) {
            if (this.innerHTML.toLowerCase() == pages[i].dataset.page) {
                pages[i].classList.add('active');
                navigationLinks[i].classList.add('active');
                window.scrollTo(0, 0);
            } else {
                pages[i].classList.remove('active');
                navigationLinks[i].classList.remove('active');
            }
        }
    });
}


$('.email').on("change keyup paste",
    function () {
        if ($(this).val()) {
            $('.icon-paper-plane').addClass("next");
        } else {
            $('.icon-paper-plane').removeClass("next");
        }
    }
);

$('.next-button').hover(
    function () {
        $(this).css('cursor', 'pointer');
    }
);

$('.next-button.email').click(
    function () {
        console.log("Something");
        $('.email-section').addClass("fold-up");
        $('.password-section').removeClass("folded");
    }
);

$('.password').on("change keyup paste",
    function () {
        if ($(this).val()) {
            $('.icon-lock').addClass("next");
        } else {
            $('.icon-lock').removeClass("next");
        }
    }
);

$('.next-button').hover(
    function () {
        $(this).css('cursor', 'pointer');
    }
);

$('.next-button.password').click(
    function () {
        console.log("Something");
        $('.password-section').addClass("fold-up");
        $('.repeat-password-section').removeClass("folded");
    }
);

$('.repeat-password').on("change keyup paste",
    function () {
        if ($(this).val()) {
            $('.icon-repeat-lock').addClass("next");
        } else {
            $('.icon-repeat-lock').removeClass("next");
        }
    }
);
$('.next-button.repeat-password').click(
    function () {
        console.log("Something");
        $('.repeat-password-section').addClass("fold-up");
        $('.success').css("marginTop", 0);
    }
);
$(document).ready(function () {
    $(".drop .option").click(function () {
        var val = $(this).attr("data-value"),
            $drop = $(".drop"),
            prevActive = $(".drop .option.active").attr("data-value"),
            options = $(".drop .option").length;

        $drop.find(".option.active").addClass("mini-hack");
        $drop.toggleClass("visible");
        $drop.removeClass("withBG");
        $(this).css("top");
        $drop.toggleClass("opacity");
        $(".mini-hack").removeClass("mini-hack");

        if ($drop.hasClass("visible")) {
            setTimeout(function () {
                $drop.addClass("withBG");
            }, 400 + options * 100);
        }

        triggerAnimation();

        if (val !== "placeholder" || prevActive === "placeholder") {
            $(".drop .option").removeClass("active");
            console.log(val);

            $(this).addClass("active");

            if (val == "deposito") {
                $('#formDeposito').css({ 'display': 'block' });
                $('#ReiniciarDepos').css({ 'visibility': 'visible' });
                $('#retiroformA').css({ 'display': 'none' });
                $('#avisoTransaccion').css({ 'display': 'none' });
            } else if (val == "retiro") {
                $('#formDeposito').css({ 'display': 'none' });
                $('#ReiniciarDepos').css({ 'visibility': 'hidden' });
                $('#avisoTransaccion').css({ 'display': 'none' });
                $('#retiroformA').css({ 'display': 'block' });
            } else if (val == "otro") {
                $('#formDeposito').css({ 'display': 'none' });
                $('#avisoTransaccion').css({ 'display': 'block' });
                $('#ReiniciarDepos').css({ 'visibility': 'hidden' });
                $('#retiroformA').css({ 'display': 'none' });
            }
        }
    });

    function triggerAnimation() {
        var finalWidth = $(".drop").hasClass("visible") ? 22 : 20;
        $(".drop").css("width", "400px");
        setTimeout(function () {
            $(".drop").css("width", finalWidth + "em");
        }, 400);
    }

    // Cerrar dropdown al hacer clic fuera
    $(document).on("click", function (e) {
        if (!$(e.target).closest(".drop").length) {
            $(".drop").removeClass("visible opacity withBG");
        }
    });
});

/*
    This pen cleverly utilizes SVG filters to create a "Morphing Text" effect. Essentially, it layers 2 text elements on top of each other, and blurs them depending on which text element should be more visible. Once the blurring is applied, both texts are fed through a threshold filter together, which produces the "gooey" effect. Check the CSS - Comment the #container rule's filter out to see how the blurring works!
*/

const elts = {
    text1: document.getElementById("text1"),
    text2: document.getElementById("text2")
};

// The strings to morph between. You can change these to anything you want!
const texts = [
    "Ingrese",
    "su",
    "Tipo",
    "de",
    "Trasancción"
];

// Controls the speed of morphing.
const morphTime = 1;
const cooldownTime = 0.25;

let textIndex = texts.length - 1;
let time = new Date();
let morph = 0;
let cooldown = cooldownTime;

elts.text1.textContent = texts[textIndex % texts.length];
elts.text2.textContent = texts[(textIndex + 1) % texts.length];

function doMorph() {
    morph -= cooldown;
    cooldown = 0;

    let fraction = morph / morphTime;

    if (fraction > 1) {
        cooldown = cooldownTime;
        fraction = 1;
    }

    setMorph(fraction);
}

// A lot of the magic happens here, this is what applies the blur filter to the text.
function setMorph(fraction) {
    // fraction = Math.cos(fraction * Math.PI) / -2 + .5;

    elts.text2.style.filter = `blur(${Math.min(8 / fraction - 8, 100)}px)`;
    elts.text2.style.opacity = `${Math.pow(fraction, 0.4) * 100}%`;

    fraction = 1 - fraction;
    elts.text1.style.filter = `blur(${Math.min(8 / fraction - 8, 100)}px)`;
    elts.text1.style.opacity = `${Math.pow(fraction, 0.4) * 100}%`;

    elts.text1.textContent = texts[textIndex % texts.length];
    elts.text2.textContent = texts[(textIndex + 1) % texts.length];
}

function doCooldown() {
    morph = 0;

    elts.text2.style.filter = "";
    elts.text2.style.opacity = "100%";

    elts.text1.style.filter = "";
    elts.text1.style.opacity = "0%";
}

// Animation loop, which is called every frame.
function animate() {
    requestAnimationFrame(animate);
    let newTime = new Date();
    let shouldIncrementIndex = cooldown > 0;
    let dt = (newTime - time) / 1000;
    time = newTime;

    cooldown -= dt;

    if (cooldown <= 0) {
        if (shouldIncrementIndex) {
            textIndex++;
        }

        doMorph();
    } else {
        doCooldown();
    }
}

// Start the animation.
animate();

