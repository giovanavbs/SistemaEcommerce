using overhaul_teste.Models;

namespace overhaul_teste.ViewModels
{
    public class CarrosViewModel
    {
        public IEnumerable<Carro> Carros { get; set; } 
        public int PaginaAtual { get; set; } 
        public int TotalPaginas { get; set; }

        // filtros
        public string Fabricante { get; set; }
        public string Modelo { get; set; } 
        public int? AnoDe { get; set; } 
        public int? AnoAte { get; set; } 
        public decimal? PrecoDe { get; set; } 
        public decimal? PrecoAte { get; set; } 

        // ordenar
        public string OrdenarPor { get; set; } 

        public IEnumerable<Carro> Top3CarrosMaisVendidos { get; set; }
    }
}
