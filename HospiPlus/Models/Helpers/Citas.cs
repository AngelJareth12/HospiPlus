using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospiPlus.Models.Helpers
{
    public class Citas
    {
        public int CitaID { get; set; }

        // Foreign Key - PacienteID INT
        public int PacienteID { get; set; }

        // Foreign Key - MedicoID INT
        public int MedicoID { get; set; }

        // FechaHora DATETIME
        public DateTime FechaHora { get; set; }

        // Motivo VARCHAR(255)
        public string Motivo { get; set; }

        // Estado VARCHAR(50)
        public string Estado { get; set; }

        // Consultorio VARCHAR(50)
        public string Consultorio { get; set; }

        // NumeroCita INT
        public int NumeroCita { get; set; }

        // Duracion INT
        public int Duracion { get; set; } // Duración en minutos, por ejemplo
    }
}