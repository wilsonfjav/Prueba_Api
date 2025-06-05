using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MovimientoEstudiantil.Models
{
    public class EstudianteDTO
    {
        [Required]
        [Column("correo")]
        [EmailAddress]
        [StringLength(100)]
        public string correo { get; set; }

        [Required]
        [Column("provincia_id")]
        public int provincia { get; set; }

        [Required]
        [Column("sede_id")]
        public int sede { get; set; }
        
        [Required]
        [Column("satisfaccion_carrera")]
        [StringLength(2)]
        public string satisfaccionCarrera { get; set; }

        [Required]
        [Column("anioIngreso")]
        public int anioIngreso { get; set; }
    }
}
