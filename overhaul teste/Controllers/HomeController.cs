using Carros.Classes.Entidades;
using Carros.infraestrutura;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using overhaul_teste.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace overhaul_teste.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Carros()
        {
            return View();
        }

        public IActionResult Detalhes()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

}
