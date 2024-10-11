namespace overhaul_teste.Models
{
    public class ItensPedido
    {
        public int IdItem { get; set; }
        public int IdCarro { get; set; }
        public string Modelo { get; set; }
        public string Marca { get; set; }
        public int Ano { get; set; }
        public decimal PrecoUnitario { get; set; }
        public int Quantidade { get; set; }
        public string Cor { get; set; }
        public string Imagem { get; set; }
    }
}
