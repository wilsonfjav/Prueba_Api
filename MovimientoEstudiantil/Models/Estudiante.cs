// Librerías para usar atributos de validación y mapeo de base de datos
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MovimientoEstudiantil.Models
{
    // Esta clase representa la tabla "Estudiante" en la base de datos
    [Table("Estudiante")]
    public class Estudiante
    {
        // Clave primaria de la tabla, se mapea a la columna "id_estudiante"
        [Key]
        [Column("id_estudiante")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]//autoincremental
        public int idEstudiante { get; set; }

        // Campo obligatorio, columna "correo", validado como dirección de correo, máx 100 caracteres
        [Required]
        [Column("correo")]
        [EmailAddress]
        [StringLength(100)]
        public string correo { get; set; }

        // Campo obligatorio, columna "provincia_id", referencia a la entidad Provincia
        [Required]
        [Column("provincia_id")]
        public int provincia { get; set; }

        // Campo obligatorio, columna "sede_id", referencia a la entidad Sede

        [Required]
        [Column("sede_id")]
        public int sede { get; set; }

        // Campo obligatorio, columna "satisfaccion_carrera", texto corto de 2 caracteres
        [Required]
        [Column("satisfaccion_carrera")]
        [StringLength(2)]
        public string satisfaccionCarrera { get; set; }

        // Campo obligatorio, columna "anioIngreso", representa el año de ingreso del estudiante
        [Required]
        [Column("anioIngreso")]
        public int anioIngreso { get; set; }

        // Propiedad de navegación: relación con la tabla Provincia (clave foránea)
        [JsonIgnore]//NO se incluye en el JSON 
        public virtual Provincia? Provincia_I { get; set; } //Se cambio 2 propiedades no pueden tener el mismo nombre aunque sea en M o m
        //ESTE CAMBIO SE DEBE DEJAR ProvinciaO NO CAMBIAR A SOLO PROVINCIA
        // Propiedad de navegación: relación con la tabla Sede (clave foránea)
        [JsonIgnore]//NO se incluye en el JSON 
        public virtual Sede? Sede_I { get; set; }

        //Se necesita el '?' para que el json no lo tome encuenta y solo lo relacione con la entidad si no genera error
    }
}