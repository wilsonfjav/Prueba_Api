namespace MovimientoEstudiantil.Models
{
    public class FiltroGraficoDTO
    {
        public int AnioInicio { get; set; }
        public int AnioFin { get; set; }
        public string Provincia { get; set; }
        public string Sede { get; set; }
        public string TrasladoResidencia { get; set; }
        public string IngresoCarreraDeseada { get; set; }
        public string TipoGrafico { get; set; } // Este SE ignorar en el backend, solo sirve para el frontend
    }

}