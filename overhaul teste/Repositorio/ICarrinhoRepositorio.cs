using overhaul_teste.Models;
using System.Collections.Generic;

namespace overhaul_teste.Repositorio
{
    public interface ICarrinhoRepositorio
    {
        void AdicionarItem(Carrinho item);
        void RemoverItem(int idCarro, int idCliente);
        List<Carrinho> ObterCarrinhoPorCliente(int idCliente);
        void AtualizarItem(Carrinho item);
        void LimparCarrinho(int idCliente);
    }
}
