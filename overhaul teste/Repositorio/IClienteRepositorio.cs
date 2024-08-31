using overhaul_teste.Models;
using MySqlX.XDevAPI;

namespace overhaul_teste.Repositorio
{
    public interface IClienteRepositorio
    {
        // crud
        //login
        Cliente Login(string Email, string Senha); // chamando a model cliente e passando os parametros para o metodo login

        // cadastrar cliente
        void Cadastrar(Cliente cliente);

    }
}
