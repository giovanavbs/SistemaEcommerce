using overhaul_teste.Models;

namespace overhaul_teste.ViewModels
{
    public class NotaFiscalViewModel
    {
        public int IdNotaFiscal { get; set; }
        public string NumeroNota { get; set; }
        public DateTime DataEmissao { get; set; }
        public decimal ValorTotal { get; set; }
        public string FormaPagamento { get; set; }
        public List<ItensPedido> Itens { get; set; } = new List<ItensPedido>();
        public string NomeCliente { get; set; }
        public string SobrenomeCliente { get; set; }
        public decimal CpfCnpj { get; set; }
    }
}
