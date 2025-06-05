// Librerías para usar atributos de validación y mapeo de base de datos
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MovimientoEstudiantil.Models
{
    [Table("Provincia")]
    public class Provincia
    {
        [Key]
        [Column("id_provincia")]
        public int idProvincia { get; set; }

        [Required]
        [StringLength(100)]
        [Column("nombre")]
        public string nombre { get; set; }
        // No se incluirá en el JSON al serializar
        // Relaciones 1:N correctamente definidas
        [JsonIgnore]
        public virtual ICollection<Sede> Sedes { get; set; }

        [JsonIgnore]
        public virtual ICollection<Estudiante> Estudiantes { get; set; }
    }
}