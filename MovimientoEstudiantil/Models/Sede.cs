using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MovimientoEstudiantil.Models
{
    [Table("Sede")]
    public class Sede
    {
        [Key]
        [Column("id_sede")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int idSede { get; set; }

        [Required]
        [Column("nombre")]
        [StringLength(100)]
        public string nombre { get; set; }

        [Required]
        [Column("provincia_id")]
        public int idProvincia { get; set; }

        // Se excluye de la serialización para evitar ciclos o payload innecesario
        [JsonIgnore]
        public virtual Provincia? Provincia { get; set; }

        [JsonIgnore]
        public virtual ICollection<Estudiante> Estudiantes { get; set; }

        [JsonIgnore]
        public virtual ICollection<Usuario> Usuarios { get; set; }
    }
}