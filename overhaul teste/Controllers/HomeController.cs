using overhaul_teste.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using overhaul_teste.Repositorio;
using overhaul_teste.Libraries.Login;
using overhaul_teste.Cookie;
using overhaul_teste.CarrinhoCompra;
using overhaul_teste.ViewModels;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.IO;
using iTextSharp.text.pdf.qrcode;
using QRCoder;
using System.Drawing;
using System;
using Font = iTextSharp.text.Font;

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
            var carrosMaisVendidos = _carroRepositorio.ObterTop3CarrosMaisVendidos();

            return View(carrosMaisVendidos);
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

            return RedirectToAction(nameof(Login));
        }

        [HttpPost]
        public IActionResult ExcluirConta()
        {
            int idCliente = Cliente.ClienteLogadoId;
            _clienteRepositorio.Excluir(idCliente);

            return RedirectToAction("Index", "Home");
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
            /*if (cliente == null)
            {
                return NotFound();
            }*/
            return View(cliente);
        }

        [HttpPost]
        public IActionResult EditarCliente(Cliente cliente)
        {
            int idCliente = Cliente.ClienteLogadoId;
            _clienteRepositorio.Atualizar(cliente, idCliente);
            return RedirectToAction("PerfilCliente");

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
            int idCliente = Cliente.ClienteLogadoId;

            if (ModelState.IsValid)
            {
                _clienteRepositorio.Atualizar(cliente, idCliente);
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
        public IActionResult SelecionarCartao(int idCartao, string cvv)
        {
            int idCliente = Cliente.ClienteLogadoId;

            // por enquanto a unica verificação feita de verdade é o do cvv ter os 3 digitos (tecnicamente nao guardamos o cvv no banco por segurança por isso n fiz a verificação se é igual como acontece no login e etc
            if (string.IsNullOrEmpty(cvv) || cvv.Length != 3)
            {
                ModelState.AddModelError("CVV", "o CVV informado é inválido");
                return View();
            }

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

        /*[HttpGet]
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
         } */

        [HttpGet]
        public IActionResult PagarPix()
        {
            int idCliente = Cliente.ClienteLogadoId;

            var pedido = _compraRepositorio.ObterPedidoRecente(idCliente);

            if (pedido != null)
            {
                string codigoPix = GerarCodigoPixAleatorio();

                _compraRepositorio.InserirFormaPagamento(pedido.IdPedido, codigoPix, pedido.ValorTotal);

                ViewBag.CodigoPix = codigoPix;

                return View("PagarPix");
            }
            else
            {
                return RedirectToAction("ErroPedido");
            }
        }

        private string GerarCodigoPixAleatorio()
        {
            Random random = new Random();
            string codigoAleatorio = $"00020126680014BR.GOV.BCB.PIX0114+5581999999995020";
            codigoAleatorio += random.Next(100000000, 999999999);
            codigoAleatorio += "5204000053039865406100.005802BR59OVERHAULSAOPAULO0001";
            codigoAleatorio += random.Next(1000, 9999);

            return codigoAleatorio;
        }


        [HttpGet]
        public IActionResult PagarBoleto()
        {
            int idCliente = Cliente.ClienteLogadoId;

            var pedido = _compraRepositorio.ObterPedidoRecente(idCliente);

            if (pedido != null)
            {
                string codigoBarras = GerarCodigoBarrasAleatorio();

                _compraRepositorio.InserirFormaPagamento(pedido.IdPedido, codigoBarras, pedido.ValorTotal);

                ViewBag.CodigoBarras = codigoBarras;

                return View("PagarBoleto");
            }
            else
            {
                return RedirectToAction("ErroPedido");
            }
        }

        private string GerarCodigoBarrasAleatorio()
        {
            Random random = new Random();
            string codigoAleatorio = "0339912345"; // começo padrao
            for (int i = 0; i < 35; i++) // total de 44 nuemros
            {
                codigoAleatorio += random.Next(0, 10).ToString();
            }
            return codigoAleatorio;
        }



        public IActionResult GerarPdfBoleto(int idPedido, string codigoBarras)
        {
            // pegando as infos do pedido
            var clienteId = Cliente.ClienteLogadoId;
            var pedido = _compraRepositorio.ObterPedidoRecente(clienteId);

            if (pedido == null)
            {
                return NotFound("Pedido não encontrado.");
            }

            // gerar o pdf
            using (var stream = new MemoryStream())
            {
                Document pdfDoc = new Document(PageSize.A4);
                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, stream);
                pdfDoc.Open();

                // pegando a logo da over
                var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "logo.png");
                if (System.IO.File.Exists(logoPath))
                {
                    var logo = iTextSharp.text.Image.GetInstance(logoPath);
                    logo.ScaleAbsoluteWidth(227);
                    logo.ScaleAbsoluteHeight(57);
                    logo.Alignment = Element.ALIGN_CENTER;
                    pdfDoc.Add(logo);
                }

                // cabeçalho bonitinho
                pdfDoc.Add(new Paragraph("OVERHAUL", FontFactory.GetFont("Arial", 14, iTextSharp.text.Font.BOLD)));
                pdfDoc.Add(new Paragraph("Boleto Bancário", FontFactory.GetFont("Arial", 10)));
                pdfDoc.Add(new Paragraph("Overhaul Concessionária de Carros Elétricos", FontFactory.GetFont("Arial", 10)));
                pdfDoc.Add(new Paragraph("Avenida Giovanni Gronchi, 2967, Morumbi, São Paulo - SP"));
                pdfDoc.Add(new Paragraph("CEP: 05010-100-SP  CNPJ: 12345678901  IE: 178290456874"));
                pdfDoc.Add(new Paragraph(new string('_', 72)));

                // infos
                pdfDoc.Add(new Paragraph($"Número do Pedido: {pedido.IdPedido}"));
                pdfDoc.Add(new Paragraph($"Data do Pedido: {pedido.DataPedido:dd/MM/yyyy}"));
                pdfDoc.Add(new Paragraph($"Valor Total: R$ {pedido.ValorTotal:F2}"));
                pdfDoc.Add(new Paragraph("Vencimento: " + DateTime.Now.AddDays(5).ToString("dd/MM/yyyy")));
                pdfDoc.Add(new Paragraph(new string('_', 72)));

                // mandando o codigo real (que tramp
                pdfDoc.Add(new Paragraph($"Código de Barras: {codigoBarras}"));

                // gerando codigo de barras
                Barcode128 barcode = new Barcode128
                {
                    Code = codigoBarras,
                    Font = null,
                    CodeType = Barcode128.CODE128,
                    BarHeight = 50f,
                    X = 1f
                };

                var barcodeImage = barcode.CreateImageWithBarcode(writer.DirectContent, null, null);
                barcodeImage.ScalePercent(150); // tamaanho
                pdfDoc.Add(barcodeImage);

                // Instruções para pagamento
                pdfDoc.Add(new Paragraph(new string('_', 72)));
                pdfDoc.Add(new Paragraph("Instruções para Pagamento:", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                pdfDoc.Add(new Paragraph("1. Copie o código de barras ou escaneie com o aplicativo do seu banco."));
                pdfDoc.Add(new Paragraph("2. Confirme as informações e finalize o pagamento."));
                pdfDoc.Add(new Paragraph("3. Confirme as informações e finalize o pagamento."));
                pdfDoc.Add(new Paragraph("4. O boleto pode levar até 3 dias para confirmar o pagamento."));
                pdfDoc.Add(new Paragraph(new string('_', 72)));

                pdfDoc.Close();

                return File(stream.ToArray(), "application/pdf", $"Boleto_{pedido.IdPedido}.pdf");
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
            int idCliente = Cliente.ClienteLogadoId;
            Cliente cliente = _clienteRepositorio.ObterCliente(idCliente);
            if (cliente == null)
            {
                return NotFound();
            }
            return View(cliente);
        }

        public IActionResult AdicionarCarro()
        {
            return View();
        }

        /* [HttpPost]
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
         } */

        // a parte de adicionar img falhou, insere o nome no bd mas nao na wwwroot
        [HttpPost]
        public IActionResult AdicionarCarro(Carro carro, IFormFile Imagem)
        {
            if (Imagem != null && Imagem.Length > 0)
            {
                string pastaImagens = Path.Combine(Directory.GetCurrentDirectory(), "carroImagens");

                if (!Directory.Exists(pastaImagens))
                {
                    Directory.CreateDirectory(pastaImagens);
                }

                var caminhoImagem = Path.Combine(pastaImagens, Imagem.FileName);
                using (var stream = new FileStream(caminhoImagem, FileMode.Create))
                {
                    Imagem.CopyTo(stream);
                }

                carro.Imagem = Imagem.FileName;
            }

            _carroRepositorio.AdicionarCarro(carro);

            TempData["MensagemSucesso"] = "Carro adicionado com sucesso!";
            return RedirectToAction("Index");
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

        public IActionResult VerTestDrives()
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

        public IActionResult VerTestDrivesCliente()
        {
            int idCliente = Cliente.ClienteLogadoId;

            var testDrives = _testDriveRepositorio.ObterTestDrivesPorCliente(idCliente);
            return View(testDrives);
        }

        [HttpPost]
        public IActionResult ConcluirTestDrive(int idTest)
        {
            _testDriveRepositorio.AtualizarStatusTestDrive(idTest, "concluido");
            return RedirectToAction("VerTestDrivesCliente");
        }

        [HttpPost]
        public IActionResult CancelarTestDrive(int idTest)
        {
            _testDriveRepositorio.AtualizarStatusTestDrive(idTest, "cancelado");
            return RedirectToAction("VerTestDrivesCliente");
        }

        [HttpGet]
        public IActionResult RegistrarAvaliacao(int idPedido)
        {

            var avaliacao = new Avaliacao
            {
                IdPedido = idPedido,
            };
            return View(avaliacao);
        }

        [HttpPost]
        public IActionResult RegistrarAvaliacao(Avaliacao avaliacao)
        {
            if (ModelState.IsValid)
            {

                _compraRepositorio.InserirAvaliacao(avaliacao);


                var detalhesAvaliacao = _compraRepositorio.ObterDetalhesAvaliacao(avaliacao.IdPedido);


                return View("DetalhesPedido", detalhesAvaliacao);
            }


            return View(avaliacao);
        }

        public IActionResult Logout()
        {
            _clienteRepositorio.Logout();
            return RedirectToAction("Index");
        }

        public IActionResult GerarNotaFiscal(int idPedido)
        {
            var pedidoDetalhes = _clienteRepositorio.ObterDetalhesPedido(idPedido);
            if (pedidoDetalhes == null)
            {
                ViewBag.MensagemErro = "nao foi possível encontrar os detalhes do pedido";
                return View("Erro");
            }

            var (idPagamento, formaPagamento) = _compraRepositorio.ObterFormaPagamentoPorPedido(idPedido);

            var notaFiscal = new NotaFiscal
            {
                IdPagamento = idPagamento,
                NumeroNota = Guid.NewGuid().ToString(),
                DataEmissao = DateTime.Now,
                ValorTotal = pedidoDetalhes.Pedido.ValorTotal,
                FormaPagamento = formaPagamento,
                Itens = new List<ItensPedido>(pedidoDetalhes.Itens)
            };

            _compraRepositorio.InserirNotaFiscal(notaFiscal);

            return ExibirNotaFiscal(idPedido);

            /*var notaFiscalSalva = _compraRepositorio.ObterNotaFiscalPorPedido(idPedido);

           
            return View("NotaFiscal", notaFiscalSalva); */
        }

        public IActionResult ExibirNotaFiscal(int idPedido)
        {

            var notaFiscal = _compraRepositorio.ObterNotaFiscalPorPedido(idPedido);

            if (notaFiscal == null)
            {
                ViewBag.MensagemErro = "nao foi possível encontrar a nota fiscal para o pedido";
                return View("Erro");
            }


            var notaFiscalViewModel = new NotaFiscalViewModel
            {
                IdNotaFiscal = notaFiscal.IdNotaFiscal,
                NumeroNota = notaFiscal.NumeroNota,
                DataEmissao = notaFiscal.DataEmissao,
                ValorTotal = notaFiscal.ValorTotal,
                FormaPagamento = notaFiscal.FormaPagamento,
                Itens = notaFiscal.Itens,
                NomeCliente = notaFiscal.NomeCliente,
                SobrenomeCliente = notaFiscal.SobrenomeCliente,
                CpfCnpj = notaFiscal.CpfCnpj
            };



            return View("NotaFiscal", notaFiscal);
        }

        public IActionResult GerarPdfNotaFiscal(int idPedido)
        {
            var notaFiscal = _compraRepositorio.ObterNotaFiscalPorPedido(idPedido);
            if (notaFiscal == null)
            {
                return NotFound("Nota fiscal não encontrada.");
            }

            var notaFiscalViewModel = new NotaFiscalViewModel
            {
                IdNotaFiscal = notaFiscal.IdNotaFiscal,
                NumeroNota = notaFiscal.NumeroNota,
                DataEmissao = notaFiscal.DataEmissao,
                ValorTotal = notaFiscal.ValorTotal,
                FormaPagamento = notaFiscal.FormaPagamento,
                Itens = notaFiscal.Itens,
                NomeCliente = notaFiscal.NomeCliente,
                SobrenomeCliente = notaFiscal.SobrenomeCliente,
                CpfCnpj = notaFiscal.CpfCnpj
            };

            using (var stream = new MemoryStream())
            {
                Document pdfDoc = new Document(PageSize.A4);
                PdfWriter.GetInstance(pdfDoc, stream);
                pdfDoc.Open();

                // parte de cima
                pdfDoc.Add(new Paragraph("OVERHAUL", FontFactory.GetFont("Arial", 14, iTextSharp.text.Font.BOLD)));
                pdfDoc.Add(new Paragraph("Overhaul Concessionária de carros elétricos", FontFactory.GetFont("Arial", 10)));
                pdfDoc.Add(new Paragraph("Avenida Giovanni Gronchi, 2967, Morumbi, São Paulo - SP"));
                pdfDoc.Add(new Paragraph("CEP: 05010-100-SP  CNPJ: 12345678901  IE: 178290456874"));
                pdfDoc.Add(new Paragraph(new string('_', 72)));

                // infos
                pdfDoc.Add(new Paragraph($"Extrato No. {notaFiscalViewModel.IdNotaFiscal}"));
                pdfDoc.Add(new Paragraph($"Número da Nota: {notaFiscalViewModel.NumeroNota}"));
                pdfDoc.Add(new Paragraph("CUPOM FISCAL ELETRÔNICO - SAT"));
                pdfDoc.Add(new Paragraph($"{notaFiscalViewModel.DataEmissao:dd/MM/yyyy}   {notaFiscalViewModel.DataEmissao:HH:mm:ss}"));
                pdfDoc.Add(new Paragraph(new string('_', 72)));

                // itens pedido
                pdfDoc.Add(new Paragraph("# | NOME | DESC | QTD | UN | VL UN R$ | (VL TR R$) * | VL ITEM R$", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                pdfDoc.Add(new Paragraph(new string('_', 72)));
                pdfDoc.Add(new Paragraph("Itens do Pedido:"));

                if (notaFiscalViewModel.Itens != null && notaFiscalViewModel.Itens.Count > 0)
                {
                    foreach (var item in notaFiscalViewModel.Itens)
                    {
                        pdfDoc.Add(new Paragraph($"{item.Quantidade} x {item.Modelo} ({item.Marca}, {item.Ano}, {item.Cor}) | Preço Unitário: R$ {item.PrecoUnitario}"));
                    }
                }
                else
                {
                    pdfDoc.Add(new Paragraph("Nenhum item encontrado para este pedido."));
                }

                // pagamento
                pdfDoc.Add(new Paragraph(new string('_', 72)));
                pdfDoc.Add(new Paragraph($"TOTAL R$ {notaFiscalViewModel.ValorTotal:F2}", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                pdfDoc.Add(new Paragraph($"{notaFiscalViewModel.FormaPagamento}   {notaFiscalViewModel.ValorTotal:F2}"));
                pdfDoc.Add(new Paragraph(new string('_', 72)));

                pdfDoc.Add(new Paragraph("Forma de Pagamento referente:", FontFactory.GetFont("Arial", 10, Font.ITALIC)));
                pdfDoc.Add(new Paragraph($"{notaFiscalViewModel.FormaPagamento}"));

                pdfDoc.Add(new Paragraph(new string('_', 72)));
                pdfDoc.Add(new Paragraph($"Operador: {notaFiscalViewModel.NomeCliente} {notaFiscalViewModel.SobrenomeCliente}"));
                pdfDoc.Add(new Paragraph($"{notaFiscalViewModel.DataEmissao:dd/MM/yyyy}   {notaFiscalViewModel.DataEmissao:HH:mm:ss}"));
                pdfDoc.Add(new Paragraph($"Pedido/Comanda: {idPedido}"));

                pdfDoc.Close();

                return File(stream.ToArray(), "application/pdf", "NotaFiscal.pdf");
            }
        }

        public IActionResult Carros(int pagina = 1, int CarrosPorPagina = 3, string pesquisa = "", string fabricante = "", string modelo = "", int? anoDe = null, int? anoAte = null, decimal? precoDe = null, decimal? precoAte = null, string ordenarPor = "")
        {
            var todosCarros = _carroRepositorio.ExibirCarros();

            if (!string.IsNullOrEmpty(pesquisa))
            {
                todosCarros = todosCarros.Where(c => c.Modelo.Contains(pesquisa, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (!string.IsNullOrEmpty(fabricante))
            {
                todosCarros = todosCarros.Where(c => c.Marca.Equals(fabricante, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (!string.IsNullOrEmpty(modelo))
            {
                todosCarros = todosCarros.Where(c => c.Modelo.Equals(modelo, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (anoDe.HasValue)
            {
                todosCarros = todosCarros.Where(c => c.Ano >= anoDe.Value).ToList();
            }

            if (anoAte.HasValue)
            {
                todosCarros = todosCarros.Where(c => c.Ano <= anoAte.Value).ToList();
            }

            if (precoDe.HasValue)
            {
                todosCarros = todosCarros.Where(c => c.Preco >= precoDe.Value).ToList();
            }

            if (precoAte.HasValue)
            {
                todosCarros = todosCarros.Where(c => c.Preco <= precoAte.Value).ToList();
            }

            switch (ordenarPor)
            {
                case "nome-asc":
                    todosCarros = todosCarros.OrderBy(c => c.Modelo).ToList();
                    break;
                case "nome-desc":
                    todosCarros = todosCarros.OrderByDescending(c => c.Marca).ToList();
                    break;
                case "preco-asc":
                    todosCarros = todosCarros.OrderBy(c => c.Preco).ToList();
                    break;
                case "preco-desc":
                    todosCarros = todosCarros.OrderByDescending(c => c.Preco).ToList();
                    break;
                case "ano-asc":
                    todosCarros = todosCarros.OrderBy(c => c.Ano).ToList();
                    break;
                case "ano-desc":
                    todosCarros = todosCarros.OrderByDescending(c => c.Ano).ToList();
                    break;
                default:
                    break;
            }

            var totalCarros = todosCarros.Count();
            var totalPaginas = (int)Math.Ceiling((double)totalCarros / CarrosPorPagina); // divisao pra saber quantas paginas pro total de carros (ceiling pra arredondar pra cima pq se a divisao der decimal ele dava um erro gigante
            // o double é pq a divisao com int tava arredondando pra baixo entao faltava uma pagina 

            var carrosPagina = todosCarros
                .Skip((pagina - 1) * CarrosPorPagina) // n repetir os carros 
                .Take(CarrosPorPagina)
                .ToList();

            // mais vendidos
            var carrosMaisVendidos = _carroRepositorio.ObterTop3CarrosMaisVendidos();

            var viewModel = new CarrosViewModel // viewmodel só pq nao da pra usar a classe carro pra exibição de partes da lista completa direto
            {
                Carros = carrosPagina,
                PaginaAtual = pagina,
                TotalPaginas = totalPaginas,
                Top3CarrosMaisVendidos = carrosMaisVendidos
            };

            return View(viewModel);
        }


    }

}


