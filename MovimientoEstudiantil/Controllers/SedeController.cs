using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovimientoEstudiantil.Data;
using MovimientoEstudiantil.Models;

namespace MovimientoEstudiantil.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SedeController : Controller
    {
        private readonly MovimientoEstudiantilContext _context;

        public SedeController(MovimientoEstudiantilContext context)
        {
            _context = context;
        }

        //------------------------------------------------------------------------//
        // GET: /Sede/Lista_Sedes
        // Retorna todas las sedes de la base de datos
        [HttpGet("ListaSedes")]
        public async Task<List<Sede>> ListaSedes()
        {
            var lista = await _context.Sedes.ToListAsync();
            return lista;
        }//end method
    }
}