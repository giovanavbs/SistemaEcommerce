using overhaul_teste.Models;
using System.Collections.Generic;

namespace overhaul_teste.ViewModels
{
    public class PagamentoConfirmadoViewModel
    {
        public Pedido Pedido { get; set; }
        public List<ItensPedido> Itens { get; set; }
        public string StatusPedido { get; set; }  

        public Endereco EnderecoEntrega { get; set; }
        public PagamentoConfirmadoViewModel()
        {
            Itens = new List<ItensPedido>();  // tava vindo a lista de itens pedido null por isso instanciei aqui
        }
    }
}
