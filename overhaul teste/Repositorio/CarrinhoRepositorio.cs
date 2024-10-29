using MySql.Data.MySqlClient;
using overhaul_teste.Models;
using System.Collections.Generic;
using System.Data;

namespace overhaul_teste.Repositorio
{
    public class CarrinhoRepositorio : ICarrinhoRepositorio
    {
        private readonly string? _conexaoMySQL;

        public CarrinhoRepositorio(IConfiguration conf) => _conexaoMySQL = conf.GetConnectionString("ConexaoMySQL");

        // adicionar ao carrinho
        public void AdicionarItem(Carrinho item)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();

                // nao adicionar 2x o mesmo item no carrinho
                var existingItem = new MySqlCommand("SELECT * FROM CarrinhoCompra WHERE id_carro = @IdCarro AND id_cliente = @IdCliente", conexao);
                existingItem.Parameters.AddWithValue("@IdCarro", item.IdCarro);
                existingItem.Parameters.AddWithValue("@IdCliente", item.IdCliente);

                using (var reader = existingItem.ExecuteReader())
                {
                    if (reader.Read())
                    {   // aumentar a quantidade se ja existe
                        var quantidadeAtual = Convert.ToInt32(reader["QuantidadeProd"]);
                        var novaQuantidade = quantidadeAtual + item.QuantidadeProd;

                        reader.Close(); 

                        var updateCmd = new MySqlCommand("UPDATE CarrinhoCompra SET QuantidadeProd = @QuantidadeProd WHERE id_carro = @IdCarro AND id_cliente = @IdCliente", conexao);
                        updateCmd.Parameters.AddWithValue("@QuantidadeProd", novaQuantidade);
                        updateCmd.Parameters.AddWithValue("@IdCarro", item.IdCarro);
                        updateCmd.Parameters.AddWithValue("@IdCliente", item.IdCliente);
                        updateCmd.ExecuteNonQuery();
                    }
                    else
                    {
                       
                        reader.Close(); 


                        var insertCmd = new MySqlCommand("INSERT INTO CarrinhoCompra (id_carro, id_cliente, modelo, marca, ano, preco, id_categoria, carregador, descricao, imagem, cor, QuantidadeProd) VALUES (@IdCarro, @IdCliente, @Modelo, @Marca, @Ano, @Preco, @IdCategoria, @Carregador, @Descricao, @Imagem, @Cor, @QuantidadeProd)", conexao);
                        insertCmd.Parameters.AddWithValue("@IdCarro", item.IdCarro);
                        insertCmd.Parameters.AddWithValue("@IdCliente", item.IdCliente);
                        insertCmd.Parameters.AddWithValue("@Modelo", item.Modelo);
                        insertCmd.Parameters.AddWithValue("@Marca", item.Marca);
                        insertCmd.Parameters.AddWithValue("@Ano", item.Ano);
                        insertCmd.Parameters.AddWithValue("@Preco", item.Preco);
                        insertCmd.Parameters.AddWithValue("@IdCategoria", 1);
                        insertCmd.Parameters.AddWithValue("@Carregador", item.Carregador);
                        insertCmd.Parameters.AddWithValue("@Descricao", item.Descricao);
                        insertCmd.Parameters.AddWithValue("@Imagem", item.Imagem);
                        insertCmd.Parameters.AddWithValue("@Cor", item.Cor);
                        insertCmd.Parameters.AddWithValue("@QuantidadeProd", item.QuantidadeProd);
                        insertCmd.ExecuteNonQuery();
                    }
                }
            }
        }



        // remover item
        public void RemoverItem(int idCarro, int idCliente)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();

                var cmd = new MySqlCommand("DELETE FROM CarrinhoCompra WHERE id_carro = @IdCarro AND id_cliente = @IdCliente", conexao);
                cmd.Parameters.AddWithValue("@IdCarro", idCarro);
                cmd.Parameters.AddWithValue("@IdCliente", idCliente);
                cmd.ExecuteNonQuery();
            }
        }

        // mostrar todos os itens do carrinho
        public List<Carrinho> ObterCarrinhoPorCliente(int idCliente)
        {
            var carrinho = new List<Carrinho>();

            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();

                var cmd = new MySqlCommand("SELECT * FROM CarrinhoCompra WHERE id_cliente = @IdCliente", conexao);
                cmd.Parameters.AddWithValue("@IdCliente", idCliente);

                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var item = new Carrinho
                        {
                            IdCliente = Convert.ToInt32(dr["id_cliente"]),
                            IdCarro = Convert.ToInt32(dr["id_carro"]),
                            Modelo = dr["modelo"].ToString(),
                            Marca = dr["marca"].ToString(),
                            Ano = Convert.ToInt32(dr["ano"]),
                            Preco = Convert.ToDecimal(dr["preco"]),
                            IdCategoria = dr["id_categoria"].ToString(),
                            Carregador = dr["carregador"].ToString(),
                            Descricao = dr["descricao"].ToString(),
                            Imagem = dr["imagem"]?.ToString(),
                            Cor = dr["cor"].ToString(),
                            QuantidadeProd = Convert.ToInt32(dr["QuantidadeProd"])
                        };
                        carrinho.Add(item);
                    }
                }
            }

            return carrinho;
        }

        // atualizar carrinho
        public void AtualizarItem(Carrinho item)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();

                var cmd = new MySqlCommand("UPDATE CarrinhoCompra SET quantidade_prod = @QuantidadeProd WHERE id_carro = @IdCarro AND id_cliente = @IdCliente", conexao);
                cmd.Parameters.AddWithValue("@QuantidadeProd", item.QuantidadeProd);
                cmd.Parameters.AddWithValue("@IdCarro", item.IdCarro);
                cmd.Parameters.AddWithValue("@IdCliente", item.IdCliente);
                cmd.ExecuteNonQuery();
            }
        }

        // limpar carrinho
        public void LimparCarrinho(int idCliente)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();

                var cmd = new MySqlCommand("DELETE FROM CarrinhoCompra WHERE id_cliente = @IdCliente", conexao);
                cmd.Parameters.AddWithValue("@IdCliente", idCliente);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
