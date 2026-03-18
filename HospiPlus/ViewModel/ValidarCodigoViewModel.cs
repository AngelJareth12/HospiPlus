using System.ComponentModel.DataAnnotations;

namespace HospiPlus.ViewModels
{
    public class ValidarCodigoViewModel
    {
        [Required]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "El código debe tener 6 dígitos")]
        public string Codigo { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
