namespace overhaul_teste.Models
{
    public class TestDrive
    {
        public int IdTest { get; set; }
        public int IdCliente { get; set; }
        public int IdCarro { get; set; }
        public DateTime DataTest { get; set; }

        // atributos do carro
        public string ModeloCarro { get; set; }
        public string MarcaCarro { get; set; }
        public string CorCarro { get; set; }
        public decimal PrecoCarro { get; set; }
    }
}
