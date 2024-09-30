using overhaul_teste.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace overhaul_teste.Models
{
    public class Carrinho
    {
        [Key]
        public int IdCliente { get; set; }

        [Key]
        public int IdCarro { get; set; }

        [Required]
        [StringLength(50)]
        public string Modelo { get; set; }

        [Required]
        [StringLength(50)]
        public string Marca { get; set; }

        [Required]
        public int Ano { get; set; }

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Preco { get; set; }

        [Required]
        public string IdCategoria { get; set; }

        [Required]
        [StringLength(50)]
        public string Carregador { get; set; }

        [Required]
        [StringLength(300)]
        public string Descricao { get; set; }

        [StringLength(200)]
        public string? Imagem { get; set; }

        [Required]
        [StringLength(20)]
        public string Cor { get; set; }

        [Required]
        public int QuantidadeProd { get; set; }

        // fks

        [ForeignKey("IdCliente")]
        public Cliente Cliente { get; set; }

        [ForeignKey("IdCategoria")]
        public Carro Categoria { get; set; }
    }
}
