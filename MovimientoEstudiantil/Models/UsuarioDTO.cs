namespace MovimientoEstudiantil.Models
{
    public class UsuarioDTO
    {
        public int idUsuario { get; set; }
        public string correo { get; set; }
        public int sede { get; set; }
        public string rol { get; set; }
        public DateTime fechaRegistro { get; set; }
    }
}
