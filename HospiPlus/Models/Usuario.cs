using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Linq;
using System.Web;
using System.Xml;

namespace HospiPlus.Models
{
    public class Usuario
    {
        public int UsuarioId { get; set; }
        public string NombreUsuario { get; set; }
        public string Contrasena { get; set; }
        public string Email { get; set; }
        public string Rol { get; set; }
        public string ImagenPerfil { get; set; }
    }
}