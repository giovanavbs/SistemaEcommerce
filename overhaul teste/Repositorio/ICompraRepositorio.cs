using overhaul_teste.Models;

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
    }
}
