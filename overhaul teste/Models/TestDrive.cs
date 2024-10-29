namespace overhaul_teste.Models
{
    public class TestDrive
    {
      /*  public int IdTest { get; set; }
        public int IdCliente { get; set; }
        public int IdCarro { get; set; }
        public DateTime DataTest { get; set; }

        // atributos do carro
        public string ModeloCarro { get; set; }
        public string MarcaCarro { get; set; }
        public string CorCarro { get; set; }
        public decimal PrecoCarro { get; set; } */

            public int IdTest { get; set; }
            public int IdCliente { get; set; }
            public int IdCarro { get; set; }
            public DateTime DataTest { get; set; }

            // cliente
            public string ClienteNome { get; set; }
            public string ClienteCpf { get; set; }

            // carro
            public string ModeloCarro { get; set; }
            public string MarcaCarro { get; set; }
            public int AnoCarro { get; set; }  
            public string CorCarro { get; set; }
            public decimal PrecoCarro { get; set; }

            public string StatusTest { get; set; }
        }
    }