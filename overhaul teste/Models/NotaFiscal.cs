namespace overhaul_teste.Models
{
    public class NotaFiscal
    {
        public int IdNotaFiscal { get; set; }
        public int IdPagamento { get; set; }
        public string NumeroNota { get; set; }
        public DateTime DataEmissao { get; set; }
        public decimal ValorTotal { get; set; }
        public string FormaPagamento { get; set; }


        // armazenar os itens do pedido na nota fiscal
        public List<ItensPedido> Itens { get; set; } = new List<ItensPedido>();

        public string NomeCliente { get; set; }
        public string SobrenomeCliente { get; set; }
        public decimal CpfCnpj { get; set; }
    }

}
