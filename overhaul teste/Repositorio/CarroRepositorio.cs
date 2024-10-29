using MySql.Data.MySqlClient;
using overhaul_teste.Models;
using System.Collections.Generic;
using System.Data;

namespace overhaul_teste.Repositorio
{
    public class CarroRepositorio : ICarroRepositorio
    {
        private readonly string? _conexaoMySQL;

        public CarroRepositorio(IConfiguration conf) => _conexaoMySQL = conf.GetConnectionString("ConexaoMySQL");

        public IEnumerable<Carro> ExibirCarros()
        {
            var carros = new List<Carro>();

            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();


                MySqlCommand cmd = new MySqlCommand("exibirCarros", conexao)
                {
                    CommandType = CommandType.StoredProcedure
                };


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
                            Carregador = Convert.ToString(dr["carregador"])  
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
                            Cor = Convert.ToString(dr["cor"]),
                            Carregador = Convert.ToString(dr["carregador"]),
                            StatusCarro = Convert.ToString(dr["status_carro"])
                        };
                    }

                    conexao.Close();
                }

                return carro;
            }
        }

        public int ObterCarroIdRecente()
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT MAX(id_carro) FROM carros", conexao);
                var result = cmd.ExecuteScalar();
                return result != DBNull.Value ? Convert.ToInt32(result) : 0;
            }
        }

        public void AdicionarCarro(Carro carro)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand("spInserirCarro", conexao)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@vModelo", carro.Modelo);
                cmd.Parameters.AddWithValue("@vMarca", carro.Marca);
                cmd.Parameters.AddWithValue("@vAno", carro.Ano);
                cmd.Parameters.AddWithValue("@vPreco", carro.Preco);
                cmd.Parameters.AddWithValue("@vIdCategoria", carro.Categoria);
                cmd.Parameters.AddWithValue("@vCarregador", carro.Carregador);
                cmd.Parameters.AddWithValue("@vDescricao", carro.Descricao);
                cmd.Parameters.AddWithValue("@vImagem", carro.Imagem);
                cmd.Parameters.AddWithValue("@vCor", carro.Cor);

                cmd.ExecuteNonQuery();
            }
        }


        public IEnumerable<Carro> ObterTodosCarros()
        {
            var carros = new List<Carro>();

            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM carros", conexao);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var carro = new Carro
                        {
                            Id = reader.GetInt32("id_carro"),
                            Modelo = reader.GetString("modelo"),
                            Marca = reader.GetString("marca"),
                            Ano = reader.GetInt32("ano"),
                            Preco = reader.GetDecimal("preco"),
                            Carregador = reader.GetString("carregador"),
                            Descricao = reader.GetString("descricao"),
                            Imagem = reader.GetString("imagem"),
                            Cor = reader.GetString("cor"),
                        };
                        carros.Add(carro);
                    }
                }
                conexao.Close();
            }

            return carros;
        }

        // metodo pro admin
        public IEnumerable<Carro> ObterTodosCarroStatus()
        {
            var carros = new List<Carro>();

            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM carros", conexao);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var carro = new Carro
                        {
                            Id = reader.GetInt32("id_carro"),
                            Modelo = reader.GetString("modelo"),
                            Marca = reader.GetString("marca"),
                            Ano = reader.GetInt32("ano"),
                            Preco = reader.GetDecimal("preco"),
                            Carregador = reader.GetString("carregador"),
                            Descricao = reader.GetString("descricao"),
                            Imagem = reader.GetString("imagem"),
                            Cor = reader.GetString("cor"),
                            StatusCarro = reader.GetString("status_carro") 
                        };
                        carros.Add(carro);
                    }
                }
                conexao.Close();
            }

            return carros;
        }

        public void ExcluirCarro(int idCarro)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand("CALL ExcluirCarro(@idCarro)", conexao);
                cmd.Parameters.AddWithValue("@idCarro", idCarro);
                cmd.ExecuteNonQuery();
                conexao.Close();
            }
        }

        public void AtualizarCarro(Carro carro)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand("spUpdateCarro", conexao);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("p_id_carro", carro.Id);
                cmd.Parameters.AddWithValue("p_modelo", carro.Modelo);
                cmd.Parameters.AddWithValue("p_marca", carro.Marca);
                cmd.Parameters.AddWithValue("p_ano", carro.Ano);
                cmd.Parameters.AddWithValue("p_preco", carro.Preco);
                cmd.Parameters.AddWithValue("p_id_categoria", carro.Categoria);
                cmd.Parameters.AddWithValue("p_carregador", carro.Carregador);
                cmd.Parameters.AddWithValue("p_descricao", carro.Descricao);
                cmd.Parameters.AddWithValue("p_imagem", carro.Imagem);
                cmd.Parameters.AddWithValue("p_cor", carro.Cor);

                cmd.ExecuteNonQuery();

                // alterando o status caso o carro estivesse excluidp
                if (carro.StatusCarro != "sob demanda")
                {
                    AlterarStatusCarro(carro.Id);
                }

                conexao.Close();
            }
        }

        public void AlterarStatusCarro(int idCarro)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();

                MySqlCommand cmd = new MySqlCommand("spAlterarStatusCarro", conexao);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("p_id_carro", idCarro);

                cmd.ExecuteNonQuery();
                conexao.Close();
            }
        }


    }
}
