using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Carros.Classes.Entidades
{
    [Table("carros")]
    public class Carro
    {
        [Key]
        public int id_carro { get; set; }

        public string modelo { get; set; }

        public string marca { get; set; }

        public int ano { get; set; }

        public decimal preco { get; set; }
    }
}
