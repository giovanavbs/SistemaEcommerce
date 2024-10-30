using overhaul_teste.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using overhaul_teste.Repositorio;
using overhaul_teste.Libraries.Login;
using overhaul_teste.Cookie;
using overhaul_teste.CarrinhoCompra;
using overhaul_teste.ViewModels;

namespace overhaul_teste.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IClienteRepositorio? _clienteRepositorio;
        private LoginCliente _loginCliente;
        private ICarroRepositorio _carroRepositorio;
        private CookieCarrinhoCompra _cookieCarrinhoCompra;
        private readonly ICarrinhoRepositorio _carrinhoRepositorio;
        private readonly ICompraRepositorio _compraRepositorio;
        private readonly ITestDriveRepositorio _testDriveRepositorio;

        public HomeController(ILogger<HomeController> logger, IClienteRepositorio clienteRepositorio, LoginCliente loginCliente, ICarroRepositorio carroRepositorio, CookieCarrinhoCompra cookieCarrinhoCompra, ICarrinhoRepositorio carrinhoRepositorio, ICompraRepositorio compraRepositorio, ITestDriveRepositorio testDriveRepositorio)
        {
            _logger = logger;
            _clienteRepositorio = clienteRepositorio;
            _loginCliente = loginCliente;
            _carroRepositorio = carroRepositorio;
            _cookieCarrinhoCompra = cookieCarrinhoCompra;
            _carrinhoRepositorio = carrinhoRepositorio;
            _compraRepositorio = compraRepositorio;
            _compraRepositorio = compraRepositorio;
            _testDriveRepositorio = testDriveRepositorio;
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
                return RedirectToAction(nameof(PerfilCliente), new { id = loginDB.Codigo });
            }
            else
            {
                ViewData["msg"] = "Senha ou usuário incorretos";
                return View();
            }
        }

        public IActionResult PerfilCliente()
        {
            int idCliente = Cliente.ClienteLogadoId;
            Cliente cliente = _clienteRepositorio.ObterCliente(idCliente);
            if (cliente == null)
            {
                return NotFound();
            }
            return View(cliente);
        }

        public IActionResult CadastrarCliente()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CadastrarCliente(Cliente cliente)
        {
            _clienteRepositorio.Cadastrar(cliente);

            return RedirectToAction(nameof(Carros));
        }

        [HttpPost]
        public IActionResult ExcluirConta()
        {
            int idCliente = Cliente.ClienteLogadoId;
            _clienteRepositorio.Excluir(idCliente);

            return RedirectToAction("Index", "Home");
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
                return NotFound();
            }
            return View(carro);
        }

        public IActionResult EditarCliente()
        {
            int idCliente = Cliente.ClienteLogadoId;
            var cliente = _clienteRepositorio.ObterCliente(idCliente);
            if (cliente == null)
            {
                return NotFound();
            }
            return View(cliente);
        }

        [HttpPost]
        public IActionResult EditarCliente(Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                _clienteRepositorio.Atualizar(cliente);
                return RedirectToAction("PerfilCliente");
            }
            return View(cliente);
        }

        // metodos de carrinho
        public IActionResult Carrinho()
        {
            int idCliente = Cliente.ClienteLogadoId;
            //int idCliente = 1; // mudar pro cliente
            var itensCarrinho = _carrinhoRepositorio.ObterCarrinhoPorCliente(idCliente);
            return View(itensCarrinho);
        }


        public IActionResult AdicionarItem(int id)
        {
            Carro carro = _carroRepositorio.ObterCarroPorId(id);

            if (carro == null)
            {
                return View("NaoExisteItem");
            }
            else
            {
                int idCliente = Cliente.ClienteLogadoId;
                //int idCliente = 1; 

                var item = new Carrinho()
                {
                    IdCarro = carro.Id,
                    IdCliente = idCliente,
                    Modelo = carro.Modelo,
                    Marca = carro.Marca,
                    Ano = carro.Ano,
                    Preco = carro.Preco,
                    IdCategoria = carro.Categoria,
                    Carregador = carro.Carregador,
                    Descricao = carro.Descricao,
                    Imagem = carro.Imagem,
                    Cor = carro.Cor,
                    QuantidadeProd = 1
                };

                _carrinhoRepositorio.AdicionarItem(item); 

                return RedirectToAction(nameof(Carrinho));
            }
        }


        public IActionResult DiminuirItem(int id)
        {
            Carro carro = _carroRepositorio.ObterCarroPorId(id);
            if (carro == null)
            {
                return View("NaoExisteItem");
            }
            else
            {
                int idCliente = Cliente.ClienteLogadoId;
                //int idCliente = 1; 

                var item = new Carrinho()
                {
                    IdCarro = carro.Id,
                    IdCliente = idCliente,
                    QuantidadeProd = 1
                };

                _carrinhoRepositorio.RemoverItem(carro.Id, idCliente); 

                return RedirectToAction(nameof(Carrinho));
            }
        }


        public IActionResult RemoverItem(int id)
        {
            int idCliente = Cliente.ClienteLogadoId;
            //int idCliente = 1; // pegar o id do cliente
            _carrinhoRepositorio.RemoverItem(id, idCliente); 
            return Json(new { success = true });
        }


        [HttpPost]
        public IActionResult FinalizarCompra()
        {
            int idCliente = Cliente.ClienteLogadoId; 
            _compraRepositorio.RegistrarPedido(idCliente);

            return RedirectToAction("ConfirmarPedido", new { idCliente });
        }


        public IActionResult ConfirmarPedido(int idCliente)
        {
            var pedido = _compraRepositorio.ObterPedidoRecente(idCliente); // pegar o pedido mais recente

            if (pedido == null)
            {
                ViewBag.MensagemErro = "Não foi possível acessar o pedido e itens.";
                return View(); 
            }

            return View(pedido); // passa o pedido e itens
        }

        [HttpPost]
        public IActionResult EditarEndereco(Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                _clienteRepositorio.Atualizar(cliente); 
                return RedirectToAction("PerfilCliente", new { id = cliente.Codigo }); 
            }

            return View(cliente); 
        }

        public IActionResult IrParaPagamento(int idPedido)
        {
            ViewBag.IdPedido = idPedido;
            return View();
        }

        public IActionResult PagarCartao(int idPedido)
        {
            int idCliente = Cliente.ClienteLogadoId;
            var cartoes = _clienteRepositorio.ObterCartoes(idCliente); 
            ViewBag.IdPedido = idPedido;

            return View(cartoes);
        }


        [HttpPost]
        public IActionResult CancelarPedido()
        {
            int idCliente = Cliente.ClienteLogadoId;
            var pedido = _compraRepositorio.ObterPedidoRecente(idCliente);

            if (pedido == null)
            {
                ViewBag.MensagemErro = "Nenhum pedido recente encontrado.";
                return View("ErroPedido");
            }



            _compraRepositorio.CancelarPedido(pedido.IdPedido, idCliente);

            // view de carros depois de cancelar o peddido
            return RedirectToAction("Carros", "Home");
        }


        [HttpPost]
        public IActionResult AdicionarCartao(int idCliente, string NumeroCartao, string NomeTitular, string Validade, string CVV, string Bandeira)
        {

            var cartao = new Cartao
            {
                IdCliente = idCliente,
                NumeroCartao = NumeroCartao,
                NomeTitular = NomeTitular,
                Validade = Validade,
                CVV = CVV,
                Bandeira = Bandeira
            };

            _clienteRepositorio.InserirCartao(cartao, idCliente);
            return RedirectToAction("PagarCartao", new { idCliente }); 
        }

        [HttpPost]
        public IActionResult SelecionarCartao(int idCartao)
        {
            int idCliente = Cliente.ClienteLogadoId;

            var pedido = _compraRepositorio.ObterPedidoRecente(idCliente);

            if (pedido != null)
            {
                _compraRepositorio.SelecionarCartao(pedido.IdPedido, idCartao, pedido.ValorTotal);

                return RedirectToAction("ConfirmarEndereco");
            }
            else
            {
                return RedirectToAction("ErroPedido");
            }
        }

        [HttpGet]
        public IActionResult PagarPix()
        {
            int idCliente = Cliente.ClienteLogadoId;

            var pedido = _compraRepositorio.ObterPedidoRecente(idCliente);

            if (pedido != null)
            {
            // gerar um codigo pix aleatorio
                Random random = new Random();
                string codigoPix = "pix=" + random.Next(100000000, 999999999).ToString() + random.Next(100000, 999999).ToString();

                _compraRepositorio.InserirFormaPagamento(pedido.IdPedido, codigoPix, pedido.ValorTotal);

                return View("PagarPix", codigoPix);
            }
            else
            {
                return RedirectToAction("ErroPedido");
            }
        }



        public IActionResult ConfirmarEndereco()
        {
            int idCliente = Cliente.ClienteLogadoId;

            var cliente = _clienteRepositorio.ObterCliente(idCliente); 
            return View(cliente); 
        }

        [HttpPost]
        public IActionResult InserirEnderecoRetirada()
        {
            try
            {

                int idCliente = Cliente.ClienteLogadoId;

                var pedido = _compraRepositorio.ObterPedidoRecente(idCliente);
                if (pedido == null)
                {
                    ViewBag.MensagemErro = "Nenhum pedido recente foi encontrado.";
                    return RedirectToAction("Erro", "Home");
                }

                int idPedido = pedido.IdPedido;


                _clienteRepositorio.InserirEnderecoRetirada(idPedido, idCliente);

                return RedirectToAction("PagamentoConfirmado", new { idCliente });
            }
            catch (Exception ex)
            {
                ViewBag.MensagemErro = "Ocorreu um erro ao inserir o endereço de retirada: " + ex.Message;
                return RedirectToAction("Erro", "Home");
            }
        }

        [HttpPost]
        public IActionResult InserirEnderecoAtual()
        {
            try
            {
                int idCliente = Cliente.ClienteLogadoId;

                var pedido = _compraRepositorio.ObterPedidoRecente(idCliente);
                if (pedido == null)
                {
                    ViewBag.MensagemErro = "Nenhum pedido recente foi encontrado.";
                    return RedirectToAction("Erro", "Home");
                }

                int idPedido = pedido.IdPedido;

                var cliente = _clienteRepositorio.ObterCliente(idCliente);

                _clienteRepositorio.InserirEnderecoAtualNaEntrega(cliente, idPedido, idCliente);

                return RedirectToAction("PagamentoConfirmado", new { idCliente });
            }
            catch (Exception ex)
            {
                ViewBag.MensagemErro = "erro ao inserir o endereço atual: " + ex.Message;
                return RedirectToAction("Erro", "Home");
            }
        }


        [HttpPost]
        public IActionResult ConfirmarEnderecoEntrega(string enderecoSelecionado, Cliente cliente)
        {
            int idCliente = Cliente.ClienteLogadoId;

            var pedido = _compraRepositorio.ObterPedidoRecente(idCliente);

            if (pedido == null)
            {
                ViewBag.MensagemErro = "nenhum pedido recente encontrado.";
                return View("ErroPedido");
            }

            if (enderecoSelecionado == "novo")
            {
                _clienteRepositorio.InserirEndereco(cliente, pedido.IdPedido, idCliente);
            }

            return RedirectToAction("PagamentoConfirmado", new { idCliente });
        }


        public IActionResult PagamentoConfirmado(int idCliente)
        {
            return ObterDetalhesPedido(idCliente); 
        }

        public IActionResult ObterDetalhesPedido(int idCliente)
        {

            var pedido = _compraRepositorio.ObterPedidoRecente(idCliente);

            if (pedido == null)
            {
                ViewBag.MensagemErro = "nao foi possível acessar o pedido e itens.";
                return View("Erro");
            }

            int idPedidoRecente = pedido.IdPedido;

            var enderecoEntrega = _compraRepositorio.ObterEnderecoEntrega(idPedidoRecente);

            var modelo = new PagamentoConfirmadoViewModel
            {
                Pedido = pedido,
                Itens = pedido.Itens,
                EnderecoEntrega = enderecoEntrega
            };

            return View("PagamentoConfirmado", modelo);
        }


        public IActionResult MarcarTestDrive(int id)
        {
            var carro = _carroRepositorio.ObterCarroPorId(id);

            if (carro == null)
            {
                return NotFound();
            }

            return View(carro);
        }

        [HttpPost]
        public IActionResult InserirTestDrive(int id_cliente, int id_carro, DateTime data_test)
        {
            _testDriveRepositorio.InserirTestDrive(id_cliente, id_carro, data_test);

            return RedirectToAction("TestDriveConfirmado");
        }


        public IActionResult TestDriveConfirmado()
        {
            int idCliente = Cliente.ClienteLogadoId;
            int idTest = _testDriveRepositorio.ObterTestDriveRecenteID(idCliente);

            var testDrive = _testDriveRepositorio.ObterTestDrive(idCliente, idTest);

            if (testDrive == null)
            {
                ViewBag.MensagemErro = "test drive nao encontrado.";
                return View("ErroTestDrive");
            }

            return View(testDrive);
        }


        // administrador

        [HttpGet]
        public IActionResult MenuAdmin()
        {
            return View();
        }

        public IActionResult AdicionarCarro()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AdicionarCarro(Carro carro)
        {
            if (ModelState.IsValid)
            {
                _carroRepositorio.AdicionarCarro(carro);
                TempData["MensagemSucesso"] = "carro adicionado com sucesso!";

                int idCarroRecente = _carroRepositorio.ObterCarroIdRecente();
                return RedirectToAction("Detalhes", "Home", new { id = idCarroRecente });
            }
            return View(carro);
        }

        public IActionResult TodosCarros()
        {
            var carros = _carroRepositorio.ObterTodosCarroStatus(); 
            return View(carros);
        }

        [HttpPost]
        public IActionResult ExcluirCarro(int idCarro)
        {
            _carroRepositorio.ExcluirCarro(idCarro);
            return RedirectToAction("TodosCarros"); 
        }

        public IActionResult EditarCarro(int id)
        {
            var carro = _carroRepositorio.ObterCarroPorId(id);
            return View(carro);
        }

        [HttpPost]
        public IActionResult EditarCarro(Carro carro)
        {
            _carroRepositorio.AtualizarCarro(carro);
            return RedirectToAction("Detalhes", new { id = carro.Id });
        }

        public IActionResult VerificarTestDrives()
        {
            var testDrives = _testDriveRepositorio.ObterTodosTestDrives();
            return View(testDrives);
        }

        public IActionResult VerificarPedidos()
        {
            var pedidos = _compraRepositorio.VerPedidos();
            return View(pedidos); 
        }

        public IActionResult VerPedidosCliente()
        {
            int idCliente = Cliente.ClienteLogadoId;

            var pedidos = _clienteRepositorio.VerPedidosCliente(idCliente);
            return View(pedidos);
        }


        public IActionResult ExibirPagamentoConfirmado(int idPedido)
        {
            var viewModel = _clienteRepositorio.ObterDetalhesPedido(idPedido);

            if (viewModel == null)
            {
                ViewBag.MensagemErro = "NAo foi possível encontrar os detalhes do pedido.";
                return View("Erro");
            }

            return View("PagamentoConfirmado", viewModel);
        }


    }

}


