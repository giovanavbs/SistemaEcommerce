using overhaul_teste.Models;
using MySqlX.XDevAPI;
using overhaul_teste.ViewModels;

namespace overhaul_teste.Repositorio
{
    public interface IClienteRepositorio
    {
        // crud
        //login
        Cliente Login(string Email, string Senha);

        // cadastrar cliente
        void Cadastrar(Cliente cliente);

        // atualizar Cliente
        void Atualizar(Cliente cliente); 


        // cliente pelo id
        Cliente ObterCliente(int id); 

        // excluir Cliente
        void Excluir(int id);

        // oegar os cartoes do cliente
        List<Cartao> ObterCartoes(int idCliente);

        // adicionar novo cartao
        void InserirCartao(Cartao cartao, int idCliente); // futuramente vai passar o id cliente como parametro

        void InserirEndereco(Cliente cliente, int idPedido, int idCliente);

        void ObterClientePorEmail(string email, Cliente cliente);

        void InserirEnderecoRetirada(int idPedido, int idCliente);

        void InserirEnderecoAtualNaEntrega(Cliente cliente, int idPedido, int idCliente);

        List<Pedido> VerPedidosCliente(int idCliente);

        PagamentoConfirmadoViewModel ObterDetalhesPedido(int idPedido);
    }
}
