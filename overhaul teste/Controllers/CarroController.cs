using Carros.infraestrutura;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace overhaul_teste.Controllers
{
    public class TesteController : Controller
    {
        private readonly CarroDbContext _context;

        public TesteController(CarroDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var carros = await _context.Carros.ToListAsync();
            return View(carros);
        }
    }
}
