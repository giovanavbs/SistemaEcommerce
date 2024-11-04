namespace overhaul_teste.Models
{
    public class Avaliacao
    {
        public int IdAvaliacao { get; set; }
        public int IdPedido { get; set; }
        public int IdCliente { get; set; }
        public string AvaliacaoEscrita { get; set; }
        public decimal AvaliacaoNota { get; set; }
        public DateTime DataAvaliacao { get; set; }

        public Avaliacao()
        {
            IdCliente = Cliente.ClienteLogadoId; 
           
        }
    }



}
