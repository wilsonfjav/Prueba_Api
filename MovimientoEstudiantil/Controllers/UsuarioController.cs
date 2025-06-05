using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovimientoEstudiantil.Data;
using MovimientoEstudiantil.Models;
using MovimientoEstudiantil.Services;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace MovimientoEstudiantil.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly MovimientoEstudiantilContext _context;
        private readonly HistorialService _historialService;

        public UsuarioController(MovimientoEstudiantilContext context, HistorialService historialService)
        {
            _context = context;
            _historialService = historialService;
        }

        [AllowAnonymous]
        [HttpGet("ListaUsuarios")]
        public async Task<ActionResult<IEnumerable<UsuarioDTO>>> GetUsuarios()
        {
            var usuarios = await _context.Usuarios
                .Select(u => new UsuarioDTO
                {
                    idUsuario = u.idUsuario,
                    correo = u.correo,
                    sede = u.sede,
                    rol = u.rol,
                    fechaRegistro = u.fechaRegistro
                })
                .ToListAsync();

            return Ok(usuarios);
        }

        [HttpGet("BuscarUsuario/{id}")]
        public async Task<ActionResult<UsuarioDTO>> BuscarUsuario(int id)
        {
            var u = await _context.Usuarios.FindAsync(id);

            if (u == null)
                return NotFound(new { message = $"Usuario con ID {id} no encontrado." });

            var usuarioDto = new UsuarioDTO
            {
                idUsuario = u.idUsuario,
                correo = u.correo,
                sede = u.sede,
                rol = u.rol,
                fechaRegistro = u.fechaRegistro
            };

            return Ok(usuarioDto);
        }

        [HttpPost("AgregarUsuario")]
        public async Task<ActionResult<UsuarioDTO>> PostUsuario([FromBody] UsuarioConUsuarioDTO dto)
        {
            var usuarioInput = dto.usuario;

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (await _context.Usuarios.AnyAsync(u => u.correo == usuarioInput.correo))
                return Conflict(new { message = "El correo ya está registrado." });

            var error = await ValidarUsuarioInputAsync(usuarioInput.contrasena, usuarioInput.sede, usuarioInput.rol);
            if (error != null)
                return BadRequest(new { message = error });

            var nuevoUsuario = new Usuario
            {
                correo = usuarioInput.correo,
                sede = usuarioInput.sede,
                contrasena = BCrypt.Net.BCrypt.HashPassword(usuarioInput.contrasena),
                rol = usuarioInput.rol,
                fechaRegistro = DateTime.UtcNow
            };

            _context.Usuarios.Add(nuevoUsuario);
            await _context.SaveChangesAsync();

            await _historialService.RegistrarAccionAsync(
                dto.idUsuario,
                "Agregar usuario",
                $"Se agregó el usuario con ID {nuevoUsuario.idUsuario}"
            );

            var dtoResponse = new UsuarioDTO
            {
                idUsuario = nuevoUsuario.idUsuario,
                correo = nuevoUsuario.correo,
                sede = nuevoUsuario.sede,
                rol = nuevoUsuario.rol,
                fechaRegistro = nuevoUsuario.fechaRegistro
            };

            return CreatedAtAction(nameof(BuscarUsuario), new { id = dtoResponse.idUsuario }, new
            {
                message = $"Usuario con ID {dtoResponse.idUsuario} creado correctamente.",
                usuario = dtoResponse
            });
        }

        [HttpPut("EditarUsuario/{id}")]
        public async Task<IActionResult> EditarUsuario(int id, [FromBody] UsuarioConUsuarioDTO dto)
        {
            var usuarioInput = dto.usuario;

            if (usuarioInput == null)
                return BadRequest(new { message = "El cuerpo de la solicitud está vacío o malformado." });

            var usuarioExistente = await _context.Usuarios.FirstOrDefaultAsync(u => u.idUsuario == id);
            if (usuarioExistente == null)
                return NotFound(new { message = $"Usuario con ID {id} no encontrado." });

            if (!await _context.Sedes.AnyAsync(s => s.idSede == usuarioInput.sede))
                return BadRequest(new { message = "La sede proporcionada no existe." });

            if (usuarioInput.rol != "Coordinador" && usuarioInput.rol != "Administrador")
                return BadRequest(new { message = "El rol solo puede ser Coordinador o Administrador." });

            if (!string.IsNullOrWhiteSpace(usuarioInput.contrasena))
            {
                if (!Regex.IsMatch(usuarioInput.contrasena, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{6,}$"))
                    return BadRequest(new { message = "La contraseña debe tener al menos 6 caracteres, incluyendo una mayúscula, una minúscula y un número." });

                usuarioExistente.contrasena = BCrypt.Net.BCrypt.HashPassword(usuarioInput.contrasena);
            }

            usuarioExistente.sede = usuarioInput.sede;
            usuarioExistente.rol = usuarioInput.rol;

            await _context.SaveChangesAsync();

            await _historialService.RegistrarAccionAsync(
                dto.idUsuario,
                "Editar usuario",
                $"Se modificó el usuario con ID {id}"
            );

            return Ok(new { message = $"Usuario con ID {id} modificado correctamente." });
        }

        [HttpDelete("EliminarUsuario{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                return NotFound(new { message = $"Usuario con ID {id} no encontrado." });

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Usuario con ID {id} eliminado correctamente." });
        }

        private async Task<string?> ValidarUsuarioInputAsync(string contrasena, int sede, string rol)
        {
            if (!await _context.Sedes.AnyAsync(s => s.idSede == sede))
                return "La sede proporcionada no existe.";

            if (rol != "Coordinador" && rol != "Administrador")
                return "El rol solo puede ser Coordinador o Administrador.";

            if (string.IsNullOrWhiteSpace(contrasena) ||
                !Regex.IsMatch(contrasena, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{6,}$"))
            {
                return "La contraseña debe tener al menos 6 caracteres, incluyendo una mayúscula, una minúscula y un número.";
            }

            return null;
        }
    }
}
