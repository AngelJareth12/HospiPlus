using HospiPlus.Models;
using HospiPlus.Models.Helpers;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace HospiPlus.Controllers
{
    public class HomeController : Controller
    {
        HospiPlusDBEntities1 db = new HospiPlusDBEntities1();
        private GeminiApiHelper _geminiHelper = new GeminiApiHelper();
        private readonly HospiPlus.ViewModel.confirmacionCita _emailService = new HospiPlus.ViewModel.confirmacionCita();

        // ... (resto de tu código del controlador) ...

        [HttpPost]
        public async Task<JsonResult> ProcesarSintomas(string sintomas)
        {
            if (!string.IsNullOrEmpty(sintomas))
            {
                // CAMBIADO: Llamamos al método de la nueva instancia de GeminiApiHelper
                var professionalSymptom = await _geminiHelper.TransformSymptomAsync(sintomas);

                return Json(new { original = sintomas, profesional = professionalSymptom });
            }
            else
            {
                return Json(new { error = "Por favor, ingresa un síntoma." });
            }
        }

        [HttpPost]
        public ActionResult CrearCita(string motivo, string fecha)
        {
            try
            {
                HospiPlus.Models.Citas ci = new HospiPlus.Models.Citas();

                // Obtener el ID del médico desde sesión
                if (Session["IdMedicoSesion"] == null || Session["idPaciente"] == null)
                {
                    return Json(new { success = false, message = "Sesión expirada o incompleta." }, JsonRequestBehavior.AllowGet);
                }

                int idMedicoSeleccionado = int.Parse(Session["IdMedicoSesion"].ToString());
                var Medico = db.Medicos.FirstOrDefault(x => x.MedicoID == idMedicoSeleccionado);
                if (Medico == null)
                {
                    return Json(new { success = false, message = "Médico no encontrado." }, JsonRequestBehavior.AllowGet);
                }

                string codPaciente = Session["idPaciente"].ToString();
                var Paciente = db.Pacientes.FirstOrDefault(x => x.PacienteID.ToString() == codPaciente);
                if (Paciente == null)
                {
                    return Json(new { success = false, message = "Paciente no encontrado." }, JsonRequestBehavior.AllowGet);
                }

                // Llenar los datos de la cita
                ci.Motivo = motivo;

                string format = "yyyy/MM/dd HH:mm";
                DateTime fechaFinal = DateTime.ParseExact(fecha, format, System.Globalization.CultureInfo.InvariantCulture);
                ci.FechaHora = fechaFinal;

                ci.MedicoID = Medico.MedicoID;
                ci.PacienteID = Paciente.PacienteID;
                ci.Estado = "Activo";
                ci.Consultorio = "NA";
                ci.Duracion = 60;

                // Manejar número de cita
                int NumCita = db.Citas.Any() ? db.Citas.Max(x => x.CitaID) : 0;
                ci.NumeroCita = NumCita + 1;

                // Agregar y guardar
                db.Citas.Add(ci);
                db.SaveChanges();
                return Json(new { success = true, message = "Cita Creada Con Éxito." }, JsonRequestBehavior.AllowGet);
            }
            catch (DbEntityValidationException ex)
            {
                var errores = new List<string>();
                foreach (var validationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        errores.Add($"Propiedad: {validationError.PropertyName}, Error: {validationError.ErrorMessage}");
                    }
                }
                return Json(new { success = false, message = "Errores de validación: " + string.Join(" | ", errores) }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Ocurrió un error inesperado: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<JsonResult> Asistencia(string mensaje)
        {
            if (!string.IsNullOrEmpty(mensaje))
            {
                // CAMBIADO: Llamamos al método de la nueva instancia de GeminiApiHelper
                var asistencia = await _geminiHelper.Asistencia(mensaje);

                return Json(new { original = mensaje, profesional = asistencia });
            }
            else
            {
                return Json(new { error = "Por favor, ingrese un mensaje." });
            }
        }
        public ActionResult Index()
        {

            if (Session["id"] != null)
            {
                System.Web.HttpContext.Current.Session["IdMedicoSesion"] = null;
                ViewBag.Modo = "desactivado";
                Session["IdMedicoSesion"] = null;
                Session["modoCita"] = null;
            
                String cod = Session["id"].ToString();
                String codPaciente = Session["idPaciente"].ToString();

                var Usuario = db.Usuarios.FirstOrDefault(x => x.UsuarioID.ToString() == cod);
                var Paciente = db.Pacientes.FirstOrDefault(x => x.PacienteID.ToString() == codPaciente);

                Usuario.NombreUsuario = Usuario.NombreUsuario.ToUpper();
                Usuario.Rol = Usuario.Rol.ToUpper();

                if (Paciente.FechaNacimiento.HasValue)
                {
                    ViewBag.FechaNacimientoFormateada = Paciente.FechaNacimiento.Value.ToString("dd/MM/yyyy");
                }
                else
                {
                  
                    ViewBag.FechaNacimientoFormateada = "Fecha no especificada";
                }


                ViewBag.fecha = DateTime.Now.ToString("dd/MM/yyyy").ToUpper().ToString();
                ViewBag.fecha += " " + DateTime.Now.ToShortTimeString().ToString().ToUpper();

                ViewBag.Usuario = Usuario;
                ViewBag.Paciente = Paciente;
                ViewBag.Medicos = ObtenerMedicos();

                   /*var depositos = ObtenerDepositosConNombre(Cuenta.ncta);
                    ViewBag.DepositosIniciales = depositos;
                    var retiros = ObtenerRetirosConNombre(Cuenta.ncta);
                    ViewBag.RetirosIniciales = retiros;
                    ViewBag.TH = transaccionesDiariasInt();
                    ViewBag.All = AllInformation();*/



                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        private List<Medico> ObtenerMedicos()
        {
            db.Configuration.LazyLoadingEnabled = false;
            db.Configuration.ProxyCreationEnabled = false;
            string[] colores = { "Red", "Pink", "Purple", "Deep-Purple", "Indigo", "Blue", 
              "Light-Blue", "Cyan", "Teal", "Green", "Light-Green", "Lime", "Yellow", "Amber",
              "Orange", "Deep-Orange", "Brown", "Grey", "Blue-Grey" };
            var Medicos = db.Medicos.ToList();

            List<Medico> lstMedicos = new List<Medico>();
            int i = 0;
            foreach (var item in Medicos)
            {
                if (i > colores.Length)
                {
                    i = 0;
                }
                    lstMedicos.Add(new Medico
                    {
                        MedicoID = item.MedicoID,
                        Nombre = item.Nombre,
                        Apellido = item.Apellido,
                        Especialida = item.Especialidad,
                        NumeroLicencia = item.NumeroLicencia,
                        Telefono = item.Telefono,
                        Email = item.Email,
                        ImagenPerfil = item.ImagenPerfil,
                        Color = colores[i]
                    });
                    i++;
            }
                 
            return lstMedicos;
        }

        public ActionResult Login()
        {
            Session["id"] = null;
            Session["IdMedicoSesion"] = null;
            ViewBag.Modo = "desactivado";
            System.Web.HttpContext.Current.Session["IdMedicoSesion"] = null;
            System.Web.HttpContext.Current.Session["modoCita"] = null;
            System.Web.HttpContext.Current.Session["id"] = null;
            ViewBag.Message = "Login";
            return View();
        }


        [HttpPost]
        public ActionResult Login(HospiPlus.Models.Usuario usuario)
        {
            if (usuario == null || string.IsNullOrEmpty(usuario.Email.ToString()) || string.IsNullOrEmpty(usuario.Contrasena))
            {
                ViewBag.Error = "Datos incompletos.";
                return View();
            }

            var usuarioIngresar = db.Usuarios.FirstOrDefault(x => x.Email == usuario.Email);
            var usuarioPaciente = db.Pacientes.FirstOrDefault(x=>x.Email == usuario.Email);
           
 
            if (usuarioIngresar != null)
            {
                if (usuario.Contrasena == usuarioIngresar.Contrasena)
                {
                    Session["id"] = usuarioIngresar.UsuarioID.ToString();
                    Session["idPaciente"] = usuarioPaciente.PacienteID.ToString();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Error = "NIT incorrecto.";
                    return View();
                }
            }
            else
            {
                ViewBag.Error = "Cliente no encontrado.";
                return View();
            }
        }

        [HttpPost]
        public ActionResult ActualizarFotoPerfil(HttpPostedFileBase imagen)
        {
            if (Session["id"] == null)
            {
                return RedirectToAction("Login");
            }

            if (imagen != null && imagen.ContentLength > 0)
            {
                try
                {
                    int usuarioId = Convert.ToInt32(Session["id"]);

                    using (var db = new HospiPlusDBEntities1())
                    {
                        var usuario = db.Usuarios.FirstOrDefault(u => u.UsuarioID == usuarioId);

                        if (usuario != null)
                        {
                            // Crear carpeta si no existe
                            string rutaCarpeta = Server.MapPath("~/Content/imagenes/");
                            if (!Directory.Exists(rutaCarpeta))
                            {
                                Directory.CreateDirectory(rutaCarpeta);
                            }

                            // Guardar archivo con nombre único para evitar conflictos
                            string extension = Path.GetExtension(imagen.FileName);
                            string nombreArchivo = "perfil_" + usuarioId + "_" + DateTime.Now.Ticks + extension;
                            string rutaCompleta = Path.Combine(rutaCarpeta, nombreArchivo);

                            imagen.SaveAs(rutaCompleta);

                            // Actualizar ruta en la base de datos
                            usuario.ImagenPerfil = "~/Content/imagenes/" + nombreArchivo;
                            db.SaveChanges();
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Manejo básico de errores
                    ViewBag.Error = "Error al subir la imagen: " + ex.Message;
                }
            }

            return RedirectToAction("Index");
        }


        public ActionResult Citas(int? idMedicoSeleccionado)
        {
            if (Session["id"] == null || idMedicoSeleccionado == null)
            {
                return RedirectToAction("Login");
            }
            else
            {
                String cod = Session["id"].ToString();
                String codPaciente = Session["idPaciente"].ToString();

                var Usuario = db.Usuarios.FirstOrDefault(x => x.UsuarioID.ToString() == cod);
                var Paciente = db.Pacientes.FirstOrDefault(x => x.PacienteID.ToString() == codPaciente);
                Usuario.Rol = Usuario.Rol.ToUpper();            
                
                Session["IdMedicoSesion"] = idMedicoSeleccionado;
               
                var Medico = db.Medicos.FirstOrDefault(x => x.MedicoID.ToString() == idMedicoSeleccionado.ToString());             
                
                ViewBag.Usuario = Usuario;
                ViewBag.Medico = Medico;
                ViewBag.Paciente = Paciente;
                ViewBag.Modo = "activo";
                return View();
            }
        }

        // GET: Home/ConfirmarCita
        public ActionResult ConfirmarCita()
        {
            if (Session["id"] == null)
            {
                return RedirectToAction("Login");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EnviarConfirmacionCita(string emailPaciente)
        {
            if (string.IsNullOrWhiteSpace(emailPaciente))
            {
                TempData["Mensaje"] = "El correo electrónico es requerido.";
                return RedirectToAction("ConfirmarCita");
            }

            try
            {
                // busca todas las citas activas del paciente con ese correo
                var citas = await db.Citas
                                  .Include(c => c.Pacientes)
                                  .Include(c => c.Medicos)
                                  .Where(c => c.Pacientes.Email == emailPaciente &&
                                            c.Estado == "Activo" &&
                                            c.FechaHora >= DateTime.Now)
                                  .OrderBy(c => c.FechaHora)
                                  .ToListAsync();

                if (!citas.Any())
                {
                    TempData["Mensaje"] = "No se encontraron citas activas para el correo proporcionado.";
                    return RedirectToAction("ConfirmarCita");
                }

                var emailService = new HospiPlus.ViewModel.confirmacionCita();
                int citasEnviadas = 0;
                var errores = new List<string>();

                foreach (var cita in citas)
                {
                    try
                    {
                        bool enviado = await emailService.EnviarConfirmacionCitaAsync(
                            cita.Pacientes.Email,
                            $"{cita.Pacientes.Nombre} {cita.Pacientes.Apellido}",
                            $"{cita.Medicos.Nombre} {cita.Medicos.Apellido}",
                            cita.FechaHora?.ToString("dd/MM/yyyy"),
                            cita.FechaHora?.ToString("hh:mm tt"),
                            cita.Consultorio,
                            cita.NumeroCita?.ToString()
                        );

                        if (enviado)
                        {
                            citasEnviadas++;
                        }
                        else
                        {
                            errores.Add($"Error al enviar confirmación para la cita #{cita.NumeroCita}");
                        }
                    }
                    catch (Exception ex)
                    {
                        errores.Add($"Error en cita #{cita.NumeroCita}: {ex.Message}");
                    }
                }

                if (citasEnviadas > 0)
                {
                    if (errores.Count == 0)
                    {
                        TempData["Mensaje"] = $"Se enviaron {citasEnviadas} confirmación(es) de cita a {emailPaciente}";
                    }
                    else
                    {
                        TempData["Mensaje"] = $"Se enviaron {citasEnviadas} de {citas.Count} confirmaciones. Algunos envíos fallaron.";
                    }
                }
                else
                {
                    TempData["Mensaje"] = "No se pudo enviar ninguna confirmación. Por favor intente más tarde.";
                }
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = $"Error: {ex.Message}";
            }

            return RedirectToAction("ConfirmarCita");
        }
    }
}