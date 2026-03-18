using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;

namespace HospiPlus.Models
{
    public class Paciente
    {
        public int PacienteID { set; get; } 
        public string Nombre { set; get; }
        public string Apellido { set; get; }
        public DateTime FechaNacimiento {  set; get; } 
        public string Genero { set; get; }
        public string Direccion { set; get; }
        public string Telefono { set; get; }
        public string Email { set; get; }
        public string ImagenPerfil { set; get; }

    }
}