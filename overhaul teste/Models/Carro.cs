namespace overhaul_teste.Models
{
public class Carro
{
    public int Id { get; set; }
    public string Modelo { get; set; }
    public string Marca { get; set; }
    public int Ano { get; set; }
    public decimal Preco { get; set; }
    public string Categoria { get; set; }

    public string Carregador { get; set; }

    public string Descricao { get; set; } 
    public string Imagem { get; set; } 
    public string Cor { get; set; }

    public string StatusCarro { get; set; }
    }

}
