using MySql.Data.MySqlClient;
using overhaul_teste.Models;
using overhaul_teste.Repositorio;
using System.Data;

public class CompraRepositorio : ICompraRepositorio
{
    private readonly string? _conexaoMySQL;

    public CompraRepositorio(IConfiguration conf) => _conexaoMySQL = conf.GetConnectionString("ConexaoMySQL");

    public void RegistrarPedido(int idCliente)
    {
        using (var connection = new MySqlConnection(_conexaoMySQL))
        {
            connection.Open();
            using (var command = new MySqlCommand("RegistrarPedido", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@p_id_cliente", idCliente);
                command.ExecuteNonQuery();
            }
        }
    }

    public Pedido ObterPedidoRecente(int idCliente)
    {
        Pedido pedido = null;

        using (var connection = new MySqlConnection(_conexaoMySQL))
        {
            connection.Open();

            string sql = "CALL spObterItensPedidoPorID(@id_cliente)";

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@id_cliente", idCliente);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        pedido = new Pedido
                        {
                            IdPedido = reader.GetInt32("id_pedido"),
                            IdCliente = reader.GetInt32("id_cliente"),
                            DataPedido = reader.GetDateTime("data_pedido"),
                            ValorTotal = reader.GetDecimal("ValorTotal"),
                            StatusPedido = reader.GetString("StatusPedido"),
                            Itens = new List<ItensPedido>()  
                        };

                        do
                        {
                            var item = new ItensPedido
                            {
                                IdItem = reader.GetInt32("id_item_pedido"),
                                IdCarro = reader.GetInt32("id_carro"),
                                Modelo = reader.GetString("modelo"),
                                Marca = reader.GetString("marca"),
                                Ano = reader.GetInt32("ano"),
                                PrecoUnitario = reader.GetDecimal("preco_unitario"),
                                Quantidade = reader.GetInt32("quantidade"),
                                Cor = reader.GetString("cor"),
                                Imagem = reader.IsDBNull(reader.GetOrdinal("imagem")) ? null : reader.GetString("imagem")
                            };

                            pedido.Itens.Add(item);
                        } while (reader.Read());
                    }
                }
            }
        }

        return pedido;
    }

    public void SelecionarCartao(int idPedido, int idCartao, decimal valorPago)
    {
        using (var connection = new MySqlConnection(_conexaoMySQL))
        {
            connection.Open();
            using (var command = new MySqlCommand("INSERT INTO FormaPagamento (id_pedido, forma_pagamento, id_cartao, valor_pago) VALUES (@idPedido, 'debito_online', @idCartao, @valorPago)", connection))
            {
                command.Parameters.AddWithValue("@idPedido", idPedido);
                command.Parameters.AddWithValue("@idCartao", idCartao);
                command.Parameters.AddWithValue("@valorPago", valorPago);
                command.ExecuteNonQuery();
            }
        }
    }

    public void InserirFormaPagamento(int idPedido, string formaPagamento, decimal valorPago)
    {
        using (var connection = new MySqlConnection(_conexaoMySQL))
        {
            connection.Open();
            using (var command = new MySqlCommand("INSERT INTO FormaPagamento (id_pedido, forma_pagamento, id_cartao, valor_pago) VALUES (@idPedido, @formaPagamento, NULL, @valorPago)", connection))
            {
                command.Parameters.AddWithValue("@idPedido", idPedido);
                command.Parameters.AddWithValue("@formaPagamento", formaPagamento);
                command.Parameters.AddWithValue("@valorPago", valorPago);
                command.ExecuteNonQuery();
            }
        }
    }

    public void CancelarPedido(int idPedido, int idCliente)
    {
        using (var connection = new MySqlConnection(_conexaoMySQL))
        {
            connection.Open();
            using (var command = new MySqlCommand("spCancelarPedido", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@p_id_pedido", idPedido);
                command.Parameters.AddWithValue("@p_id_cliente", idCliente);

                command.ExecuteNonQuery();
            }
        }
    }


    public Endereco ObterEnderecoEntrega(int idPedido)
    {
        Endereco endereco = null;

        using (var connection = new MySqlConnection(_conexaoMySQL))
        {
            connection.Open();

            string sql = "CALL spObterEnderecoEntregaPorPedido(@id_pedido)";

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@id_pedido", idPedido);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        endereco = new Endereco
                        {
                            IdCliente = reader.GetInt32("id_cliente"), 
                            Logradouro = reader.GetString("logradouro"),
                            CEP = reader.GetInt32("CEP"),
                            BairroNome = reader.GetString("BairroNome"), 
                            CidadeNome = reader.GetString("CidadeNome"), 
                            UFNome = reader.GetString("UFNome") 
                        };
                    }
                }
            }
        }

        return endereco;
    }


    public List<Pedido> VerPedidos()
    {
        var pedidos = new List<Pedido>();

        using (var connection = new MySqlConnection(_conexaoMySQL))
        {
            connection.Open();

            using (var command = new MySqlCommand("ObterTodosPedidosEItens", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var pedido = new Pedido
                        {
                            IdPedido = reader.GetInt32("id_pedido"),
                            DataPedido = reader.GetDateTime("data_pedido"),
                            ValorTotal = reader.GetDecimal("valor_total"),
                            StatusPedido = reader.GetString("status_pedido"),
                            NomeCliente = reader.GetString("nome"),
                            SobrenomeCliente = reader.GetString("sobrenome"),
                            Itens = new List<ItensPedido>()
                        };

                        if (!reader.IsDBNull(reader.GetOrdinal("id_item_pedido")))
                        {
                            var item = new ItensPedido
                            {
                                IdItem = reader.GetInt32("id_item_pedido"),
                                Modelo = reader.GetString("modelo"),  
                                Marca = reader.GetString("marca"),
                                Quantidade = reader.GetInt32("quantidade"),
                                PrecoUnitario = reader.GetDecimal("preco_unitario"),
                            };
                            pedido.Itens.Add(item);
                        }

                        pedidos.Add(pedido);
                    }
                }
            }
        }

        return pedidos;
    }

    public Pedido ObterPedido(int idPedido)
    {
        Pedido pedido = null;

        using (var connection = new MySqlConnection(_conexaoMySQL))
        {
            connection.Open();

            string sql = "CALL spObterItensPedidoPorID2(@id_pedido)";

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@id_pedido", idPedido);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        pedido = new Pedido
                        {
                            IdPedido = reader.GetInt32("id_pedido"),
                            IdCliente = reader.GetInt32("id_cliente"),
                            DataPedido = reader.GetDateTime("data_pedido"),
                            ValorTotal = reader.GetDecimal("ValorTotal"),
                            StatusPedido = reader.GetString("StatusPedido"),
                            Itens = new List<ItensPedido>()
                        };

                        do
                        {
                            var item = new ItensPedido
                            {
                                IdItem = reader.GetInt32("id_item_pedido"),
                                IdCarro = reader.GetInt32("id_carro"),
                                Modelo = reader.GetString("modelo"),
                                Marca = reader.GetString("marca"),
                                Ano = reader.GetInt32("ano"),
                                PrecoUnitario = reader.GetDecimal("preco_unitario"),
                                Quantidade = reader.GetInt32("quantidade"),
                                Cor = reader.GetString("cor"),
                                Imagem = reader.IsDBNull(reader.GetOrdinal("imagem")) ? null : reader.GetString("imagem")
                            };

                            pedido.Itens.Add(item);
                        } while (reader.Read());
                    }
                }
            }
        }

        return pedido;
    }

}
