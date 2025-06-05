using Microsoft.AspNetCore.Mvc;
using MovimientoEstudiantil.Data;
using MovimientoEstudiantil.Models;
using MovimientoEstudiantil.Services;

namespace MovimientoEstudiantil.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GraficoApiController : ControllerBase
    {
        private readonly MovimientoEstudiantilContext _context;
        private readonly HistorialService _historialService;

        public GraficoApiController(MovimientoEstudiantilContext context, HistorialService historialService)
        {
            _context = context;
            _historialService = historialService;
        }

        // POST: api/GraficoApi/FiltrarGrafico
        [HttpPost("FiltrarGrafico")]
        public IActionResult FiltrarGrafico([FromBody] FiltroGraficoDTO filtro)
        {
            try
            {
                var query = _context.Estudiantes
                    .Where(e => e.Sede_I != null) // nos aseguramos que la sede esté cargada
                    .AsQueryable();

                // Filtro por año de ingreso
                if (filtro.AnioInicio > 0 && filtro.AnioFin > 0)
                {
                    query = query.Where(e => e.anioIngreso >= filtro.AnioInicio && e.anioIngreso <= filtro.AnioFin);
                }

                // Filtro por provincia
                if (!string.IsNullOrEmpty(filtro.Provincia))
                {
                    query = query.Where(e => e.Provincia_I != null && e.Provincia_I.nombre == filtro.Provincia);
                }

                // Filtro por sede
                if (!string.IsNullOrEmpty(filtro.Sede))
                {
                    query = query.Where(e => e.Sede_I != null && e.Sede_I.nombre == filtro.Sede);
                }

                // Filtro por traslado de residencia (calculado)
                if (!string.IsNullOrEmpty(filtro.TrasladoResidencia))
                {
                    if (filtro.TrasladoResidencia == "Sí")
                    {
                        query = query.Where(e => e.Sede_I.idProvincia != e.provincia);
                    }
                    else if (filtro.TrasladoResidencia == "No")
                    {
                        query = query.Where(e => e.Sede_I.idProvincia == e.provincia);
                    }
                }

                // Filtro por ingreso a carrera deseada
                if (!string.IsNullOrEmpty(filtro.IngresoCarreraDeseada))
                {
                    query = query.Where(e => e.satisfaccionCarrera == (filtro.IngresoCarreraDeseada == "Sí" ? "SI" : "NO"));
                }

                // Agrupación por satisfacción carrera como ejemplo
                var resultado = query
                    .GroupBy(e => e.satisfaccionCarrera)
                    .Select(g => new GraficoDTO
                    {
                        Categoria = g.Key,
                        Cantidad = g.Count()
                    })
                    .ToList();

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error en FiltrarGrafico: {ex.Message}");
                return StatusCode(500, "Error interno del servidor");
            }
        }

    }
}