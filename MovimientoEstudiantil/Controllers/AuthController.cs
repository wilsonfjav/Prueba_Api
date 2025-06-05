using BCrypt.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using MovimientoEstudiantil.Data;
using MovimientoEstudiantil.Models;
using MovimientoEstudiantil.Services;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MovimientoEstudiantil.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly MovimientoEstudiantilContext _context;
        private readonly HistorialService _historialService;

        public AuthController(MovimientoEstudiantilContext context, HistorialService historialService)
        {
            _context = context;
            _historialService = historialService;
        }

        // POST: api/Auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Datos inválidos" });

            // Busca al usuario por correo (el correo sí está en texto plano)
            var usuario = _context.Usuarios.FirstOrDefault(u => u.correo == model.Correo);

            // Si no existe o la contraseña no es válida
            if (usuario == null || !BCrypt.Net.BCrypt.Verify(model.Contrasena, usuario.contrasena))
                return Unauthorized(new { message = "Correo o contraseña incorrectos" });

            // Crear claims para el usuario
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.idUsuario.ToString()),
                new Claim(ClaimTypes.Name, usuario.correo),
                new Claim(ClaimTypes.Role, usuario.rol) // Muy importante para autorizar por rol
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            // Crear el principal
            var principal = new ClaimsPrincipal(claimsIdentity);

            // Firmar sesión con cookie
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            await _historialService.RegistrarAccionAsync(
                usuario.idUsuario,
                "Login",
                $"Usuario {usuario.correo} inició sesión."
            );

            // Usuario autenticado correctamente
            return Ok(new
            {
                id = usuario.idUsuario,
                correo = usuario.correo,
                rol = usuario.rol
            });
        }

        // POST: api/Auth/logout
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim != null)
            {
                int idUsuario = int.Parse(userIdClaim.Value);
                await _historialService.RegistrarAccionAsync(
                    idUsuario,
                    "Logout",
                    $"Usuario con ID {idUsuario} cerró sesión."
                );
            }

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            // Aquí podrías eliminar token, limpiar sesión, etc.
            return Ok(new { message = "Sesión cerrada correctamente." });
        }
    }
}
