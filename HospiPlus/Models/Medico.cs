using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospiPlus.Models
{
    public class Medico
    {
        public int MedicoID { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Especialida { get; set; }
        public string NumeroLicencia { get; set; }
        public string Telefono { get; set; }
        public string Email {  get; set; }
        public string ImagenPerfil { get; set; }
        public string Color { get; set; }

        public  Medico()
        {

        }
    }
}