using System.ComponentModel.DataAnnotations;

namespace MovimientoEstudiantil.Models
{
    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public string Correo { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Contrasena { get; set; }
    }
}
