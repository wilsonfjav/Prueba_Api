using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovimientoEstudiantil.Data;
using MovimientoEstudiantil.Models;
using MovimientoEstudiantil.Services;
using System.Text.Json;

namespace MovimientoEstudiantil.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EstudianteController : ControllerBase
    {
        private readonly MovimientoEstudiantilContext _context;
        private readonly HistorialService _historialService;

        public EstudianteController(MovimientoEstudiantilContext context, HistorialService historialService)
        {
            _context = context;
            _historialService = historialService;
        }

        [HttpGet("ListaEstudiantes")]
        public async Task<List<Estudiante>> ListaEstudiantes()
        {
            return await _context.Estudiantes.ToListAsync();
        }

        [HttpGet("BuscarEstudiante/{id}")]
        public async Task<IActionResult> Buscar(int id)
        {
            var estudiante = await _context.Estudiantes.FirstOrDefaultAsync(e => e.idEstudiante == id);
            if (estudiante == null)
                return NotFound($"No existe ningún estudiante con el ID {id}.");
            return Ok(estudiante);
        }

        [HttpDelete("EliminarEstudiante/{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var estudiante = await _context.Estudiantes.FindAsync(id);
            if (estudiante == null)
                return NotFound(new { message = $"No existe el estudiante con ID {id}" });

            _context.Estudiantes.Remove(estudiante);
            await _context.SaveChangesAsync();

            // Historial sin usuario autenticado
            await _historialService.RegistrarAccionAsync(0, "Eliminar estudiante", $"Se eliminó el estudiante con ID {id}");

            return Ok(new { message = $"Estudiante {id} eliminado correctamente" });
        }

        [HttpPost("AgregarEstudiante")]
        public async Task<IActionResult> AgregarEstudiante([FromBody] EstudianteConUsuarioDTO dto)
        {
            try
            {
                // Validación inicial
                if (dto == null || dto.estudiante == null)
                    return BadRequest(new { message = "Debe enviar un objeto estudiante válido." });

                var estudianteDTO = dto.estudiante;
                int idUsuario = dto.idUsuario;

                var estudiante = new Estudiante
                {
                    correo = estudianteDTO.correo?.Trim(),
                    provincia = estudianteDTO.provincia,
                    sede = estudianteDTO.sede,
                    satisfaccionCarrera = estudianteDTO.satisfaccionCarrera?.Trim(),
                    anioIngreso = estudianteDTO.anioIngreso
                };

                // Validar correo duplicado en el mismo año
                bool correoExistente = await _context.Estudiantes.AnyAsync(u =>
                    u.correo == estudiante.correo &&
                    u.anioIngreso == estudiante.anioIngreso);

                if (correoExistente)
                    return Conflict(new { message = "El correo ya está registrado para ese año de ingreso." });

                // Validación personalizada
                var error = await ValidarEstudianteAsync(estudiante);
                if (!string.IsNullOrEmpty(error))
                    return BadRequest(new { message = error });

                // Guardar
                _context.Estudiantes.Add(estudiante);
                await _context.SaveChangesAsync();

                // Registrar en el historial
                await _historialService.RegistrarAccionAsync(
                    idUsuario,
                    "Agregar estudiante",
                    $"Se agregó el estudiante con ID {estudiante.idEstudiante}"
                );

                return Ok(new { message = $"Estudiante {estudiante.idEstudiante} fue almacenado correctamente." });
            }
            catch (JsonException ex)
            {
                return BadRequest(new { message = "Error de formato en los datos enviados.", detail = ex.Message });
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new { message = "Error al guardar en la base de datos.", detail = ex.InnerException?.Message ?? ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno en el servidor.", detail = ex.Message });
            }
        }



        [HttpPut("ModificarEstudiante/{id}")]
        public async Task<IActionResult> Modificar(int id, [FromBody] EstudianteDTO dto)
        {
            if (dto == null)
                return BadRequest(new { message = "Debe enviar un objeto estudiante válido." });

            var estudiante = await _context.Estudiantes.FirstOrDefaultAsync(e => e.idEstudiante == id);
            if (estudiante == null)
                return NotFound(new { message = $"No existe el estudiante con ID {id}" });

            if (await _context.Estudiantes.AnyAsync(e => e.correo == dto.correo && e.anioIngreso == dto.anioIngreso && e.idEstudiante != id))
                return Conflict(new { message = "El correo ya está registrado para ese año de ingreso." });

            estudiante.correo = dto.correo;
            estudiante.provincia = dto.provincia;
            estudiante.sede = dto.sede;
            estudiante.satisfaccionCarrera = dto.satisfaccionCarrera;
            estudiante.anioIngreso = dto.anioIngreso;

            var error = await ValidarEstudianteAsync(estudiante);
            if (error != null)
                return BadRequest(new { message = error });

            await _context.SaveChangesAsync();

            // Historial sin usuario autenticado
            await _historialService.RegistrarAccionAsync(0, "Modificar estudiante", $"Se modificó el estudiante con ID {id}");

            return Ok(new { message = $"Estudiante {id} modificado correctamente." });
        }

        private async Task<string?> ValidarEstudianteAsync(Estudiante estudiante)
        {
            if (string.IsNullOrWhiteSpace(estudiante.correo) || !estudiante.correo.EndsWith("@ucr.ac.cr"))
                return "Error: Solo se permiten correos del dominio '@ucr.ac.cr'.";

            if (string.IsNullOrWhiteSpace(estudiante.satisfaccionCarrera) ||
                !(new[] { "si", "no" }).Contains(estudiante.satisfaccionCarrera.Trim().ToLower()))
                return "Error: La satisfacción debe ser 'Si' o 'No'.";

            if (!(new[] { 2022, 2023, 2024 }).Contains(estudiante.anioIngreso))
                return "Error: El año de ingreso debe ser 2022, 2023 o 2024.";

            if (!await _context.Provincias.AnyAsync(p => p.idProvincia == estudiante.provincia))
                return $"Error: La provincia con ID {estudiante.provincia} no existe.";

            if (!await _context.Sedes.AnyAsync(s => s.idSede == estudiante.sede))
                return $"Error: La sede con ID {estudiante.sede} no existe.";

            return null;
        }
    }
}
