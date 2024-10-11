namespace overhaul_teste.Models
{
        public class Pedido
        {
            public int IdPedido { get; set; }
            public int IdCliente { get; set; }
            public DateTime DataPedido { get; set; }
            public decimal ValorTotal { get; set; }

           public string StatusPedido { get; set; }

        public List<ItensPedido> Itens { get; set; } = new List<ItensPedido>();

    }
}
