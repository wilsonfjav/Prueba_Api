using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovimientoEstudiantil.Models
{
    public class HistorialRegistro
    {
        [Key]
        [Column("id_historial")]
        public int idHistorial { get; set; }

        [Required]
        [Column("usuario_id")]
        public int idUsuario { get; set; }

        [Required]
        [StringLength(25)]
        public string accion { get; set; }

        [StringLength(200)]
        public string descripcion { get; set; }

        [Required]
        [Column("fecha_registro", TypeName = "date")]
        public DateTime fechaRegistro { get; set; } = DateTime.Now;

        // En tu tabla no hay columna para hora, si quieres conservarla deberías añadirla en SQL
        [Required]
        [Column("hora_registro", TypeName = "time")]
        public TimeSpan hora { get; set; } = DateTime.Now.TimeOfDay;

        // Propiedad de navegación
        [ForeignKey("idUsuario")]
        public virtual Usuario Usuario { get; set; }
    }
}
