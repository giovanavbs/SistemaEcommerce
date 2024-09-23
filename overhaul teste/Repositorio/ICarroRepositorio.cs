using overhaul_teste.Models;

namespace overhaul_teste.Repositorio
{
    public interface ICarroRepositorio
    {
        // Método para exibir todos os carros
        IEnumerable<Carro> ExibirCarros();

        // Método para obter um carro por ID
        Carro ObterCarroPorId(int id);

    }
}

