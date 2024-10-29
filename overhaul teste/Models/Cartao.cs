using System;

namespace overhaul_teste.Models
{
    public class Cartao
    {
        public int IdCartao { get; set; } 
        public int IdCliente { get; set; } 
        public string NumeroCartao { get; set; } 
        public string NomeTitular { get; set; }
        public string Validade { get; set; }
        public string CVV { get; set; } 
        public string Bandeira { get; set; } 

        // ultimos 4 digitos do cartao
        public string UltimosQuatroDigitos
        {
            get
            {
                return NumeroCartao.Length > 4 ? NumeroCartao.Substring(NumeroCartao.Length - 4) : NumeroCartao;
            }
        }
    }
}
