using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovimientoEstudiantil.Data;
using MovimientoEstudiantil.Models;
using MovimientoEstudiantil.Services;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MovimientoEstudiantil.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HistorialRegistroController : ControllerBase
    {
        private readonly MovimientoEstudiantilContext _context;
        private readonly HistorialService _historialService;

        public HistorialRegistroController(MovimientoEstudiantilContext context, HistorialService historialService)
        {
            _context = context;
            _historialService = historialService;
        }

        //------------------------------------------------------------------------//
        // GET: /HistorialRegistro/ListaHistorial
        // Retorna todas las entradas del historial de todos los usuarios
        [HttpGet("ListaHistorial")]
        public async Task<IActionResult> ListaHistorial()
        {
            var lista = await _context.HistorialRegistros
                .OrderByDescending(h => h.fechaRegistro)
                .ThenByDescending(h => h.hora)
                .Select(h => new
                {
                    h.idHistorial,
                    h.idUsuario,
                    h.accion,
                    h.descripcion,
                    h.fechaRegistro,
                    h.hora
                })
                .ToListAsync();

            return Ok(lista);
        }

        // GET: /HistorialRegistro/ListaHistorialUsuario/{usuarioId}
        // Retorna todas las entradas del historial para un usuario concreto
        [HttpGet("BuscarHistorial/{usuarioId}")]
        public async Task<IActionResult> ListaHistorialUsuario(int usuarioId)
        {
            var lista = await _context.HistorialRegistros
                .Where(h => h.idUsuario == usuarioId)
                .OrderByDescending(h => h.fechaRegistro)
                .ThenByDescending(h => h.hora)
                .Select(h => new
                {
                    h.idHistorial,
                    h.idUsuario,
                    h.accion,
                    h.descripcion,
                    h.fechaRegistro,
                    h.hora
                })
                .ToListAsync();

            if (lista.Count == 0)
                return NotFound(new { message = $"No se encontraron registros de historial para el usuario con ID {usuarioId}." });

            return Ok(lista);
        }

        // POST: /HistorialRegistro/AgregarHistorial
        [HttpPost("AgregarHistorial")]
        public async Task<IActionResult> AgregarHistorial([FromBody] CrearHistorialDTO dto)
        {
            if (dto == null)
                return BadRequest(new { message = "Debe enviar un objeto válido." });

            if (!await _context.Usuarios.AnyAsync(u => u.idUsuario == dto.IdUsuario))
                return NotFound(new { message = $"Usuario con ID {dto.IdUsuario} no encontrado." });

            await _historialService.RegistrarAccionAsync(
                dto.IdUsuario,
                dto.Accion,
                dto.Descripcion
            );

            return Ok(new
            {
                message = "Entrada de historial creada correctamente",
                datos = new
                {
                    dto.IdUsuario,
                    dto.Accion,
                    dto.Descripcion
                }
            });
        }

        // DELETE: /HistorialRegistro/EliminarHistorial/{id}
        [HttpDelete("EliminarHistorial/{id}")]
        public async Task<IActionResult> EliminarHistorial(int id)
        {
            var entrada = await _context.HistorialRegistros.FindAsync(id);
            if (entrada == null)
                return NotFound(new { message = $"Entrada de historial con ID {id} no encontrada." });

            _context.HistorialRegistros.Remove(entrada);
            await _context.SaveChangesAsync();

            // Registrar en historial quién eliminó este registro
            var idUsuarioStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(idUsuarioStr) || !int.TryParse(idUsuarioStr, out var idUsuario))
                return Unauthorized(new { message = "Usuario no autenticado o inválido." });

            await _historialService.RegistrarAccionAsync(
                idUsuario,
                "Eliminar historial",
                $"Se eliminó la entrada de historial con ID {id}"
            );

            return Ok(new { message = $"Entrada de historial con ID {id} eliminada correctamente." });
        }

    }
}
