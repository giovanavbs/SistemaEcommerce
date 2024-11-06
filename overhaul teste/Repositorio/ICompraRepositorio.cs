using overhaul_teste.Models;
using overhaul_teste.ViewModels;

namespace overhaul_teste.Repositorio
{
    public interface ICompraRepositorio
    {
        void RegistrarPedido(int idCliente);

        Pedido ObterPedidoRecente(int idCliente);

        void SelecionarCartao(int idPedido, int idCartao, decimal valorPago);

        Endereco ObterEnderecoEntrega(int idPedido);

        void InserirFormaPagamento(int idPedido, string formaPagamento, decimal valorPago);

        void CancelarPedido(int idPedido, int idCliente);

        List<Pedido> VerPedidos();

        public Pedido ObterPedido(int idPedido);

        // void InserirAvaliacao(int idPedido, int idCliente, string avaliacaoEscrita, decimal avaliacaoNota);

        void InserirAvaliacao(Avaliacao avaliacao);

        //List<AvaliacaoViewModel> ObterAvaliacaoPorPedido(int idPedido);

        AvaliacaoViewModel ObterDetalhesAvaliacao(int idPedido);

        void InserirNotaFiscal(NotaFiscal notaFiscal);

        (int IdPagamento, string FormaPagamento) ObterFormaPagamentoPorPedido(int idPedido);

        NotaFiscal ObterNotaFiscalPorPedido(int idPedido);
    }
}
