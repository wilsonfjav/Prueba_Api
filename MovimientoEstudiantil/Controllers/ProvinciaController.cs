using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovimientoEstudiantil.Data;
using MovimientoEstudiantil.Models;

namespace MovimientoEstudiantil.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProvinciaController : Controller
    {
        private readonly MovimientoEstudiantilContext _context;

        public ProvinciaController(MovimientoEstudiantilContext context)
        {
            _context = context;
        }

        //------------------------------------------------------------------------//
        // GET: /Provincia/Lista_Provincias
        // Retorna todas las provincias de la base de datos
        [HttpGet("ListaProvincias")]
        public async Task<List<Provincia>> ListaProvincias()
        {
            var lista = await _context.Provincias.ToListAsync();
            return lista;
        }//end method
    }
}