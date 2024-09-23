using overhaul_teste.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using overhaul_teste.Repositorio;
using overhaul_teste.Libraries.Login;

namespace overhaul_teste.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger; 
                                                        
                                                       

        private IClienteRepositorio? _clienteRepositorio; 
        private LoginCliente _loginCliente;

        private ICarroRepositorio _carroRepositorio;

        public HomeController(ILogger<HomeController> logger, IClienteRepositorio clienteRepositorio, LoginCliente loginCliente, ICarroRepositorio carroRepositorio)
        {
            _logger = logger;
            _clienteRepositorio = clienteRepositorio;
            _loginCliente = loginCliente;
            _carroRepositorio = carroRepositorio;
        }

        public IActionResult Index() 
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(Cliente cliente)
        {
            Cliente loginDB = _clienteRepositorio.Login(cliente.Email, cliente.Senha);

            if (loginDB.Email != null && loginDB.Senha != null)
            {
                _loginCliente.Login(loginDB);
                return new RedirectResult(Url.Action(nameof(Carros)));
            }
            else
            {
                ViewData["msg"] = "Senha ou Usuario incorretos"; 
                return View();
            }

        }

        public IActionResult CadastrarCliente()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CadastrarCliente(Cliente cliente)
        {
            _clienteRepositorio.Cadastrar(cliente);

            //redireciona para PainelCliente
            return RedirectToAction(nameof(Carros)); 
        }

        public IActionResult Carros()
        {
            var carros = _carroRepositorio.ExibirCarros();
            return View(carros);
        }

        public IActionResult Detalhes(int id)
        {
            var carro = _carroRepositorio.ObterCarroPorId(id);
            if (carro == null)
            {
                return NotFound(); // se nao achar o carro
            }
            return View(carro);
        }


    }

}
