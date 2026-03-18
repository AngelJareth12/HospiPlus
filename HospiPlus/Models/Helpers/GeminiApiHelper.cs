using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace HospiPlus.Models.Helpers
{
    public class GeminiApiHelper
    {
        // ¡IMPORTANTE! Reemplaza esto con tu clave de API de Gemini de Google AI Studio
        private readonly string _geminiApiKey = "AIzaSyBeqUfmDYGr0Dydx_MDB2PtSljZNw8gOQI";

        // Puedes usar "gemini-pro" para texto general, o "gemini-1.0-pro" o "gemini-1.5-flash"
        private readonly string _modelId = "gemini-1.5-flash";

        public async Task<string> TransformSymptomAsync(string patientSymptom)
        {
            using (var httpClient = new HttpClient())
            {
                // La API de Gemini usa la clave en la URL, no en el encabezado Authorization
                var apiUrl = $"https://generativelanguage.googleapis.com/v1beta/models/{_modelId}:generateContent?key={_geminiApiKey}";

                // Estructura del payload para la API de Gemini
                var payload = new
                {
                    contents = new[]
                    {
                        new
                        {
                            role = "user",
                            parts = new[]
                            {
                                    new { text = $"Transforme la descripción del síntoma del paciente a un lenguaje médico profesional y detallado, enfocándose en la semiología y la presentación clínica objetiva resumida lo mas posible que no exceda a las 200 caracteres. El resultado no debe exceder los 255 caracteres. Síntoma: \"{patientSymptom}\"" }
                            }
                        }
                    }
                };

                var jsonPayload = JsonConvert.SerializeObject(payload);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                try
                {
                    var response = await httpClient.PostAsync(apiUrl, content);
                    Console.WriteLine($"Código de estado de la respuesta de Gemini: {response.StatusCode}");

                    // Si la respuesta no es exitosa, intentamos leer el contenido del error
                    if (!response.IsSuccessStatusCode)
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"Contenido del error de la API de Gemini: {errorContent}");
                        response.EnsureSuccessStatusCode(); // Esto lanzará la excepción HttpRequestException
                    }

                    var rawResponse = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Respuesta cruda de la API de Gemini (éxito): {rawResponse}");

                    // Deserializar la respuesta de Gemini
                    dynamic geminiResult = JsonConvert.DeserializeObject(rawResponse);

                    // Extraer el texto generado. La estructura de Gemini es diferente a Hugging Face.
                    // Accedemos a candidates[0].content.parts[0].text
                    if (geminiResult?.candidates != null && geminiResult.candidates.Count > 0 &&
                        geminiResult.candidates[0].content?.parts != null && geminiResult.candidates[0].content.parts.Count > 0)
                    {
                        return geminiResult.candidates[0].content.parts[0].text;
                    }
                    else
                    {
                        Console.WriteLine("La respuesta de Gemini no contiene el texto generado esperado.");
                        return null;
                    }
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"Error al llamar a la API de Gemini: {ex.Message}");
                    return null;
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"Error al deserializar la respuesta de Gemini: {ex.Message}");
                    return null;
                }
                catch (Exception ex) // Captura cualquier otra excepción inesperada
                {
                    Console.WriteLine($"Ocurrió una excepción inesperada: {ex.Message}");
                    return null;
                }
            }
        }


        public async Task<string> Asistencia(string mensaje)
        {
            using (var httpClient = new HttpClient())
            {
                // La API de Gemini usa la clave en la URL, no en el encabezado Authorization
                var apiUrl = $"https://generativelanguage.googleapis.com/v1beta/models/{_modelId}:generateContent?key={_geminiApiKey}";

                // Estructura del payload para la API de Gemini
                var payload = new
                {
                    contents = new[]
                    {
                        new
                        {
                            role = "user",
                            parts = new[]
                            {
                                new { text = $"Actúa como Aquile, un asistente virtual cálido y empático en la app Hospi Plus que guía a los pacientes a agendar citas desde la app, escucha sus síntomas con" +
    $" empatía sin recetar medicamentos y ofrece recomendaciones seguras (reposo, hidratación, buena" +
    $" alimentación, etc.); sugiere la especialidad adecuada según los síntomas (dermatología para piel, " +
    $"ortopedia/reumatología para dolor muscular, gastroenterología para malestar estomacal, neumología o " +
    $"medicina general para problemas respiratorios, psicología/psiquiatría para ansiedad o estrés, y " +
    $"medicina general para dudas generales o cuando no den detalles), informa que cada cita dura 1 hora" +
    $" y que cada doctor tiene un límite de 10 citas diarias, brinda ayuda paso a paso si tienen problemas " +
    $"al agendar y responde siempre con calidez, sin repetir el mensaje de bienvenida que ya fue enviado." +

    $" Si el paciente desea simplemente seguir los pasos para agendar una cita sin recibir valoración, puedes guiarlo también." +
    $" La app tiene varias secciones: 'Home' muestra información llamativa de la empresa y valoraciones de otros pacientes." +
    $" La sección del asistente (Aquile) es donde tú interactúas para brindar recomendaciones, sugerencias y límites de citas," +
    $" como que no puede agendar una cita con el mismo doctor si ya tiene una programada." +
    $" También sirves como soporte emocional empático en caso de que lo necesite." +

    $" En la sección de 'Doctores', los pacientes pueden seleccionar un especialista, y si no lo encuentran rápido," +
    $" pueden usar un filtro por nombre, apellido o especialidad. Una vez seleccionado el doctor, aparecerá un botón para ir a los pasos de creación de la cita." +

    $" En esa página se muestra un calendario donde debe seleccionar una fecha posterior al día actual. No se permite agendar citas para el mismo día o días anteriores." +
    $" Luego debe elegir una hora entre las 6:00 a.m. y 10:00 p.m. Si ya hay una cita agendada con ese doctor en esa fecha y hora, no se podrá crear la nueva cita." +
    $" Después, el paciente debe escribir el motivo de la cita, y se recomienda que sea lo más detallado posible para que el doctor entienda mejor la situación." +
    $" Finalmente, se mostrará un resumen con la información de la cita y, al presionar 'Crear cita', se indicará si el proceso fue exitoso o no." +
    $" \r\n: \"{mensaje}\"" } }
                        }
                    }
                };

                var jsonPayload = JsonConvert.SerializeObject(payload);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                try
                {
                    var response = await httpClient.PostAsync(apiUrl, content);
                    Console.WriteLine($"Código de estado de la respuesta de Gemini: {response.StatusCode}");

                    // Si la respuesta no es exitosa, intentamos leer el contenido del error
                    if (!response.IsSuccessStatusCode)
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"Contenido del error de la API de Gemini: {errorContent}");
                        response.EnsureSuccessStatusCode(); // Esto lanzará la excepción HttpRequestException
                    }

                    var rawResponse = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Respuesta cruda de la API de Gemini (éxito): {rawResponse}");

                    // Deserializar la respuesta de Gemini
                    dynamic geminiResult = JsonConvert.DeserializeObject(rawResponse);

                    // Extraer el texto generado. La estructura de Gemini es diferente a Hugging Face.
                    // Accedemos a candidates[0].content.parts[0].text
                    if (geminiResult?.candidates != null && geminiResult.candidates.Count > 0 &&
                        geminiResult.candidates[0].content?.parts != null && geminiResult.candidates[0].content.parts.Count > 0)
                    {
                        return geminiResult.candidates[0].content.parts[0].text;
                    }
                    else
                    {
                        Console.WriteLine("La respuesta de Gemini no contiene el texto generado esperado.");
                        return null;
                    }
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"Error al llamar a la API de Gemini: {ex.Message}");
                    return null;
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"Error al deserializar la respuesta de Gemini: {ex.Message}");
                    return null;
                }
                catch (Exception ex) // Captura cualquier otra excepción inesperada
                {
                    Console.WriteLine($"Ocurrió una excepción inesperada: {ex.Message}");
                    return null;
                }
            }
        }


    }
}