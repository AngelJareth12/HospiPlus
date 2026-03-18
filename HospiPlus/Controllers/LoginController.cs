using System;
using System.Collections.Generic;
using System.IO;
using HospiPlus.ViewModels;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HospiPlus.Models;

namespace HospiPlus.Controllers
{
    public class LoginController : Controller
    {
        HospiPlusDBEntities1 db = new HospiPlusDBEntities1();

        public ActionResult RecuperarContrasena()
        {
            return View();
        }

        // POST: Solicitar envío de código de recuperación
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult EnviarCodigoRecuperacion(string email)
        {
            // Buscar en la tabla Pacientes en lugar de Usuarios
            var paciente = db.Pacientes.FirstOrDefault(p => p.Email == email);
            if (paciente == null)
            {
                ViewBag.ErrorRecuperar = "No se encontró un paciente con ese correo.";
                return View("RecuperarContrasena");
            }

            // Generar código de 6 dígitos
            var codigo = new Random().Next(100000, 999999).ToString();

            // Guardar código y email en sesión
            Session["CodigoRecuperacion"] = codigo;
            Session["EmailRecuperacion"] = email;

            // Enviar correo con código
            try
            {
                var fromAddress = new MailAddress("soporte.hospiplus@gmail.com", "HospiPlus Soporte");
                var toAddress = new MailAddress(email);
                const string fromPassword = "wohj ujgx kvzz lhqj";
                const string subject = "Código de recuperación de contraseña HospiPlus";

                // Cargar la imagen del logo
                string logoRutaCompleta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content", "usersImg", "hospiplus_logo.jpg");
                LinkedResource logoLinkedResource = new LinkedResource(logoRutaCompleta, "image/jpg");
                logoLinkedResource.ContentId = "hospiplus_logo";

                // Obtener nombre completo del paciente
                string nombreCompleto = $"{paciente.Nombre} {paciente.Apellido}";

                // Construir el cuerpo HTML con el nuevo diseño
                string cuerpoHtml = $@"
<!DOCTYPE html>
<html>
<body style=""font-family:Arial, sans-serif;line-height:1.6;color:#e0e0e0;background-color:#1a1a1a;"">
    <div class='container' style=""max-width:600px;margin:20px auto;padding:20px;border:1px solid #333;border-radius:8px;background-color:#000000;"">
        <div class='header' style=""background-color:#ffd700; color:#000000; padding:10px 20px; text-align:center; border-radius:8px 8px 0 0; border-bottom:2px solid #000000;"">
            <h2 style=""color:#000000;"">Recuperación de contraseña HospiPlus</h2>
        </div>
        <div class='logo' style=""text-align:center;margin-top:20px;"">
            <img src='cid:hospiplus_logo' alt='Logo HospiPlus' style=""max-width:180px;height:auto;"">
        </div>
        <div class='content' style=""padding:20px;color:#e0e0e0;"">
            <p style=""color:#ffffff;"">Estimado/a, {nombreCompleto}:</p>
            <p style=""color:#ffffff;"">Hemos recibido una solicitud para restablecer la contraseña de su cuenta en HospiPlus.</p>
            <p style=""color:#ffffff;"">Su código de recuperación es:</p>
            <div class='code' style=""font-size:28px;font-weight:bold;color:#ffd700;text-align:center;margin:25px 0;padding:15px;background-color:#333;border-radius:5px;letter-spacing:3px;"">{codigo}</div>
            <p style=""color:#ffffff;"">Por favor, ingrese este código en la página de recuperación de contraseña para establecer una nueva. Este código es válido por un tiempo limitado.</p>
            <p style=""color:#ffffff;"">Si usted no solicitó esta recuperación, por favor ignore este correo electrónico. Por su seguridad, <b>no comparta este código con nadie</b>.</p>
            <p style=""color:#ffffff;"">Si tiene alguna pregunta, no dude en contactarnos.</p>
        </div>
        <div class='footer' style=""text-align:center;font-size:0.9em;color:#e0e0e0;margin-top:25px;padding-top:15px;border-top:1px solid #333;background-color:#000000;padding-bottom:10px;border-radius:0 0 8px 8px;"">
            <p>Atentamente,<br>El equipo de HospiPlus</p>
            <p>&copy; {DateTime.Now.Year} HospiPlus. Todos los derechos reservados.</p>
        </div>
    </div>
</body>
</html>";

                // Crear la vista alternativa HTML con la imagen incrustada
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(cuerpoHtml, null, "text/html");
                htmlView.LinkedResources.Add(logoLinkedResource);

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword),
                    Timeout = 20000,
                };

                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    AlternateViews = { htmlView }
                })
                {
                    smtp.Send(message);
                }

                // Liberar recursos
                logoLinkedResource.Dispose();
                htmlView.Dispose();

                TempData["Mensaje"] = "Código de recuperación enviado. Revisa tu correo.";
                return RedirectToAction("ValidarCodigo");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorRecuperar = "Error al enviar correo: " + ex.Message;
                return View("RecuperarContrasena");
            }
        }

        // GET: Mostrar formulario para validar código de recuperación
        [AllowAnonymous]
        public ActionResult ValidarCodigo()
        {
            return View();
        }

        // POST: Validar código ingresado
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult ValidarCodigo(string[] codigoIngresado, string email)
        {
            if (codigoIngresado == null || codigoIngresado.Length != 6)
            {
                ModelState.AddModelError("", "Debes ingresar el código completo de 6 dígitos.");

                // Retornar el modelo con el email
                return View(new ValidarCodigoViewModel { Email = email });
            }

            string codigoCompleto = string.Concat(codigoIngresado);

            string codigoSession = Session["CodigoRecuperacion"] as string;

            if (codigoSession == null)
            {
                TempData["Error"] = "No hay un código de recuperación activo. Solicita uno.";
                return RedirectToAction("Login");
            }

            if (codigoCompleto == codigoSession)
            {
                return RedirectToAction("CambiarContrasena");
            }
            else
            {
                ModelState.AddModelError("", "Código incorrecto. Intenta de nuevo.");
                return View(new ValidarCodigoViewModel { Email = email });
            }
        }

        // GET: Mostrar formulario para cambiar contraseña
        [HttpGet]
        [AllowAnonymous]
        public ActionResult CambiarContrasena()
        {
            var email = Session["EmailRecuperacion"] as string;

            if (string.IsNullOrEmpty(email))
            {
                TempData["MensajeRecuperacion"] = "Debes ingresar tu correo para cambiar la contraseña.";
                return RedirectToAction("Login", "Home");
            }

            var model = new CambiarContrasenaViewModel
            {
                Email = email
            };

            return View(model);
        }

        // POST: Cambiar contraseña
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult CambiarContrasena(CambiarContrasenaViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (model.NuevaContrasena != model.ConfirmarContrasena)
            {
                ModelState.AddModelError("", "Las contraseñas no coinciden.");
                return View(model);
            }

            var usuario = db.Usuarios.FirstOrDefault(u => u.Email == model.Email);

            if (usuario == null)
            {
                TempData["Error"] = "Usuario no encontrado.";
                return RedirectToAction("Login", "Home");
            }

            // Aquí deberías hashear la contraseña antes de guardarla
            usuario.Contrasena = model.NuevaContrasena;

            db.SaveChanges();

            // Limpiar sesión
            Session.Remove("EmailRecuperacion");
            Session.Remove("CodigoRecuperacion");

            TempData["Exito"] = "Contraseña cambiada correctamente. Ya puedes iniciar sesión.";
            return RedirectToAction("Login", "Home");
        }
    }
}
