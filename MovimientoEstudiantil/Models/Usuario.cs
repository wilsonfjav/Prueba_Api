using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovimientoEstudiantil.Models
{
    [Table("Usuario")]
    public class Usuario
    {
        [Key]
        [Column("id_usuario")]
        public int idUsuario { get; set; }

        [Required]
        [Column("sede_id")]
        public int sede { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        [Column("correo")]
        public string correo { get; set; }

        [Required]
        [StringLength(255)]
        [Column("contrasena")]
        public string contrasena { get; set; }

        [Required]
        [StringLength(20)]
        [Column("rol")]
        public string rol { get; set; }

        [Required]
        [Column("fecha_registro", TypeName = "date")]
        public DateTime fechaRegistro { get; set; }

        public virtual Sede Sede { get; set; } // Si tienes una clase Sede definida

    }
}
