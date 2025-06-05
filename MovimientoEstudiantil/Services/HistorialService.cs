using MovimientoEstudiantil.Data;
using MovimientoEstudiantil.Models;
using System;
using System.Threading.Tasks;

namespace MovimientoEstudiantil.Services
{
    public class HistorialService
    {
        private readonly MovimientoEstudiantilContext _context;

        public HistorialService(MovimientoEstudiantilContext context)
        {
            _context = context;
        }

        public async Task RegistrarAccionAsync(int idUsuario, string accion, string descripcion = null)
        {
            var historial = new HistorialRegistro
            {
                idUsuario = idUsuario,
                accion = accion,
                descripcion = descripcion,
                fechaRegistro = DateTime.Now.Date,
                hora = DateTime.Now.TimeOfDay
            };

            _context.HistorialRegistros.Add(historial);
            await _context.SaveChangesAsync();
        }
    }
}
