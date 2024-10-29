using overhaul_teste.Models;

namespace overhaul_teste.Repositorio
{
    public interface ICarroRepositorio
    {
        IEnumerable<Carro> ExibirCarros();

        Carro ObterCarroPorId(int id);

        int ObterCarroIdRecente();

        void AdicionarCarro(Carro carro);

        IEnumerable<Carro> ObterTodosCarros();

        IEnumerable<Carro> ObterTodosCarroStatus();

        void ExcluirCarro(int idCarro);

        void AtualizarCarro(Carro carro);
    }
}

