using MySql.Data.MySqlClient;
using overhaul_teste.Models;
using System.Collections.Generic;
using System.Data;

namespace overhaul_teste.Repositorio
{
    // Implementação da interface de Carro
    public class CarroRepositorio : ICarroRepositorio
    {
        private readonly string? _conexaoMySQL;

        // Construtor para inicializar a string de conexão
        public CarroRepositorio(IConfiguration conf) => _conexaoMySQL = conf.GetConnectionString("ConexaoMySQL");

        // Método para exibir todos os carros utilizando a procedure exibirCarros
        public IEnumerable<Carro> ExibirCarros()
        {
            var carros = new List<Carro>();

            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();

                // Chama a procedure para exibir os carros
                MySqlCommand cmd = new MySqlCommand("exibirCarros", conexao)
                {
                    CommandType = CommandType.StoredProcedure
                };

                // Executa a leitura dos dados
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var carro = new Carro
                        {
                            Id = Convert.ToInt32(dr["id_carro"]),
                            Modelo = Convert.ToString(dr["modelo"]),
                            Marca = Convert.ToString(dr["marca"]),
                            Ano = Convert.ToInt32(dr["ano"]),
                            Preco = Convert.ToDecimal(dr["preco"]),
                            Categoria = Convert.ToString(dr["categoria"]),
                        };

                        carros.Add(carro);
                    }
                }

                conexao.Close();
            }

            return carros;
        }

        public Carro ObterCarroPorId(int id)
        {
            Carro carro = null;

            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();

                MySqlCommand cmd = new MySqlCommand("exibirDetalhesCarro", conexao)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@p_id_carro", id);

                using (var dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        carro = new Carro
                        {
                            Id = Convert.ToInt32(dr["id_carro"]),
                            Modelo = Convert.ToString(dr["modelo"]),
                            Marca = Convert.ToString(dr["marca"]),
                            Ano = Convert.ToInt32(dr["ano"]),
                            Preco = Convert.ToDecimal(dr["preco"]),
                            Categoria = Convert.ToString(dr["categoria"]),
                            Descricao = Convert.ToString(dr["descricao"]),
                            Imagem = Convert.ToString(dr["imagem"]),
                            Cor = Convert.ToString(dr["cor"])
                        };
                    }

                    conexao.Close();
                }

                return carro;
            }

        }
    }
}

