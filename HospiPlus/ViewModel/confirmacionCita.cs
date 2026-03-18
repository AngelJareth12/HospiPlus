using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;

namespace HospiPlus.ViewModel
{
    public class confirmacionCita
    {
        private readonly string senderEmail = "soporte.hospiplus@gmail.com";
        private readonly string appPassword = "wohj ujgx kvzz lhqj";

        private readonly string smtpHost = "smtp.gmail.com";
        private readonly int smtpPort = 587;

        public async Task<bool> EnviarConfirmacionCitaAsync(
            string to,
            string pacienteNombre,
            string medicoNombre,
            string fechaCita,
            string horaCita,
            string consultorio,
            string numeroCita)
        {
            try
            {
                ValidateEmail(to);

                string logoRutaCompleta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content", "Images", "hospiplus_logo.jpg");

                LinkedResource logoLinkedResource = null;
                AlternateView htmlView = null;

                try
                {
                    logoLinkedResource = new LinkedResource(logoRutaCompleta, "image/jpg");
                    logoLinkedResource.ContentId = "hospiplus_logo";

                    // Plantilla HTML del correo de confirmación de cita
                    string cuerpoHtml = $@"
                    <!DOCTYPE html>
                    <html>
                    <body style=""font-family:Arial, sans-serif;line-height:1.6;color:#e0e0e0;background-color:#1a1a1a;"">
                        <div class='container' style=""max-width:600px;margin-top:20px;margin-bottom:20px;margin-right:auto;margin-left:auto;padding-top:20px;padding-bottom:20px;padding-right:20px;padding-left:20px;border-width:1px;border-style:solid;border-color:#333;border-radius:8px;background-color:#000000;"">
                            <div class='header' style=""background-color:#ffd700; color:#000000; padding-top:10px; padding-bottom:10px; padding-right:20px; padding-left:20px; text-align:center; border-radius:8px 8px 0 0; border-bottom-width:2px; border-bottom-style:solid; border-bottom-color:#000000;"">
                                <h2 style=""color:#000000;"">Confirmación de cita HospiPlus</h2>
                            </div>
                            <div class='logo' style=""text-align:center;margin-top:20px;"">
                                <img src='cid:hospiplus_logo' alt='Logo HospiPlus' style=""max-width:180px;height:auto;"">
                            </div>
                            <div class='content' style=""padding-top:20px;padding-bottom:20px;padding-right:20px;padding-left:20px;color:#e0e0e0;"">
                                <p>Estimado/a, <b>{pacienteNombre}</b>:</p>
                                <p>¡Tu cita en HospiPlus ha sido confirmada con éxito!</p>

                                <h3 style=""color:#ffd700; text-align:center; margin-top:30px; margin-bottom:15px;"">Detalles de tu cita</h3>
                                <table style=""width:100%; border-collapse:collapse; margin-bottom:20px;"">
                                    <tr>
                                        <td style=""padding:8px; border:1px solid #333; background-color:#222; color:#ffd700; font-weight:bold;"">Paciente:</td>
                                        <td style=""padding:8px; border:1px solid #333; background-color:#222;"">{pacienteNombre}</td>
                                    </tr>
                                    <tr>
                                        <td style=""padding:8px; border:1px solid #333; background-color:#222; color:#ffd700; font-weight:bold;"">Médico:</td>
                                        <td style=""padding:8px; border:1px solid #333; background-color:#222;"">{medicoNombre}</td>
                                    </tr>
                                    <tr>
                                        <td style=""padding:8px; border:1px solid #333; background-color:#222; color:#ffd700; font-weight:bold;"">Fecha:</td>
                                        <td style=""padding:8px; border:1px solid #333; background-color:#222;"">{fechaCita}</td>
                                    </tr>
                                    <tr>
                                        <td style=""padding:8px; border:1px solid #333; background-color:#222; color:#ffd700; font-weight:bold;"">Hora:</td>
                                        <td style=""padding:8px; border:1px solid #333; background-color:#222;"">{horaCita}</td>
                                    </tr>
                                    <tr>
                                        <td style=""padding:8px; border:1px solid #333; background-color:#222; color:#ffd700; font-weight:bold;"">Consultorio:</td>
                                        <td style=""padding:8px; border:1px solid #333; background-color:#222;"">{consultorio}</td>
                                    </tr>
                                    <tr>
                                        <td style=""padding:8px; border:1px solid #333; background-color:#222; color:#ffd700; font-weight:bold;"">Número de cita (ID):</td>
                                        <td style=""padding:8px; border:1px solid #333; background-color:#222;"">{numeroCita}</td>
                                    </tr>
                                </table>

                                <h3 style=""color:#ffd700; margin-top:30px; margin-bottom:10px;"">Qué esperar de tu cita:</h3>
                                <ul style=""list-style:none; padding:0; margin:0;"">
                                    <li style=""margin-bottom:8px;""><span style=""color:#ffd700; font-weight:bold;"">&#10003;</span> Atención personalizada y de calidad.</li>
                                    <li style=""margin-bottom:8px;""><span style=""color:#ffd700; font-weight:bold;"">&#10003;</span> Profesionales médicos altamente calificados.</li>
                                    <li style=""margin-bottom:8px;""><span style=""color:#ffd700; font-weight:bold;"">&#10003;</span> Un ambiente seguro y cómodo.</li>
                                </ul>

                                <h3 style=""color:#ffd700; margin-top:30px; margin-bottom:10px;"">Preparación para tu cita:</h3>
                                <ul style=""list-style:none; padding:0; margin:0;"">
                                    <li style=""margin-bottom:8px;""><span style=""color:#ffd700; font-weight:bold;"">&#9200;</span> Llegar 15 minutos antes de la hora programada.</li>
                                    <li style=""margin-bottom:8px;""><span style=""color:#ffd700; font-weight:bold;"">&#128179;</span> Traer tu documento de identificación.</li>
                                    <li style=""margin-bottom:8px;""><span style=""color:#ffd700; font-weight:bold;"">&#128138;</span> Una lista de todos los medicamentos que tomas actualmente.</li>
                                    <li style=""margin-bottom:8px;""><span style=""color:#ffd700; font-weight:bold;"">&#128220;</span> Cualquier historial médico relevante o resultados de exámenes previos.</li>
                                </ul>

                                <h3 style=""color:#ffd700; margin-top:30px; margin-bottom:10px;"">Contacto y soporte:</h3>
                                <p>Si tienes alguna pregunta o necesitas reagendar tu cita, no dudes en contactarnos. Estamos aquí para ayudarte.</p>
                                <p style=""margin-bottom:5px;""><b>Teléfono:</b> +503 2243-2424</p>
                                <p><b>Email:</b> <a href=""mailto:soporte.hospiplus@gmail.com"" style=""color: #e0e0e0 !important; text-decoration: none !important; mso-style-priority:100 !important;"">soporte.hospiplus@gmail.com</a></p>
                            </div>
                            <div class='footer' style=""text-align:center;font-size:0.9em;color:#e0e0e0;margin-top:25px;padding-top:15px;border-top-width:1px;border-top-style:solid;border-top-color:#333;background-color:#000000;padding-bottom:10px;border-radius:0 0 8px 8px;"">
                                <p>Atentamente,<br>El equipo de HospiPlus</p>
                                <p>&copy; 2025 HospiPlus. Todos los derechos reservados.</p>
                            </div>
                        </div>
                    </body>
                    </html>";

                    htmlView = AlternateView.CreateAlternateViewFromString(cuerpoHtml, null, "text/html");
                    htmlView.LinkedResources.Add(logoLinkedResource);

                    using (var mail = new MailMessage(senderEmail, to))
                    {
                        mail.Subject = "Confirmación de cita - HospiPlus";
                        mail.AlternateViews.Add(htmlView);

                        using (SmtpClient smtp = new SmtpClient())
                        {
                            smtp.Host = smtpHost;
                            smtp.Port = smtpPort;
                            smtp.EnableSsl = true;
                            smtp.UseDefaultCredentials = false;
                            smtp.Credentials = new NetworkCredential(senderEmail, appPassword);
                            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

                            await smtp.SendMailAsync(mail);
                        }
                    }
                }
                finally
                {
                    logoLinkedResource?.Dispose();
                    htmlView?.Dispose();
                }

                return true;
            }
            catch (SmtpException ex)
            {
                Console.WriteLine($"SMTP Error al enviar confirmación de cita: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error general al enviar confirmación de cita: {ex.Message}");
                return false;
            }
        }

        private void ValidateEmail(string email)
        {
            var addr = new MailAddress(email);
        }
    }

    public class EmailResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
