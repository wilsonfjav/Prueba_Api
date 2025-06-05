using System.ComponentModel.DataAnnotations;

namespace MovimientoEstudiantil.Models
{
    public class UsuarioCreateDTO
    {
        [Required]
        public int sede { get; set; }

        [Required]
        [EmailAddress]
        [RegularExpression(@"^[^@\s]+@ucr\.ac\.cr$", ErrorMessage = "Solo se permiten correos @ucr.ac.cr")]
        public string correo { get; set; }

        [Required]
        [MinLength(6)]
        public string contrasena { get; set; }

        [Required]
        public string rol { get; set; }
    }
}
