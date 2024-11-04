using overhaul_teste.Models;

namespace overhaul_teste.ViewModels
{
        public class AvaliacaoViewModel
        {
            public int IdAvaliacao { get; set; }
            public string NomeCliente { get; set; }
            public string SobrenomeCliente { get; set; }
            public string AvaliacaoEscrita { get; set; }
            public decimal AvaliacaoNota { get; set; }
            public DateTime DataAvaliacao { get; set; }
            public int Quantidade { get; set; }
            public string MarcaCarro { get; set; }
            public string ModeloCarro { get; set; }
            public int AnoCarro { get; set; }

        public List<ItensPedido> ItensPedido { get; set; }
    }
    }

