using MySql.Data.MySqlClient;
using overhaul_teste.Models;
using overhaul_teste.Repositorio;
using overhaul_teste.ViewModels;
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

    public void InserirAvaliacao(Avaliacao avaliacao)
    {
        using (var connection = new MySqlConnection(_conexaoMySQL))
        {
            connection.Open();

            string sql = "CALL spInserirAvaliacao(@id_pedido, @id_cliente, @avaliacao_escrita, @avaliacao_nota)";

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@id_pedido", avaliacao.IdPedido);
                command.Parameters.AddWithValue("@id_cliente", avaliacao.IdCliente);
                command.Parameters.AddWithValue("@avaliacao_escrita", avaliacao.AvaliacaoEscrita);
                command.Parameters.AddWithValue("@avaliacao_nota", avaliacao.AvaliacaoNota);

                command.ExecuteNonQuery();
            }
        }
    }


    public AvaliacaoViewModel ObterDetalhesAvaliacao(int idPedido)
    {
        using (var connection = new MySqlConnection(_conexaoMySQL))
        {
            connection.Open();
            string sql = "CALL spObterAvaliacaoPorPedido(@id_pedido)";

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@id_pedido", idPedido);

                using (var reader = command.ExecuteReader())
                {
                    
                    var avaliacao = new AvaliacaoViewModel();
                    avaliacao.ItensPedido = new List<ItensPedido>();

                    while (reader.Read())
                    {
                        if (avaliacao.IdAvaliacao == 0) 
                        {
                            avaliacao.IdAvaliacao = reader.GetInt32("id_avaliacao");
                            avaliacao.NomeCliente = reader.GetString("nome");
                            avaliacao.SobrenomeCliente = reader.GetString("sobrenome");
                            avaliacao.AvaliacaoEscrita = reader.GetString("avaliacao_escrita");
                            avaliacao.AvaliacaoNota = reader.GetDecimal("avaliacao_nota");
                            avaliacao.DataAvaliacao = reader.GetDateTime("data_avaliacao");
                        }

 
                        var item = new ItensPedido
                        {
                            Quantidade = reader.GetInt32("quantidade"),
                            Marca = reader.GetString("marca"),
                            Modelo = reader.GetString("modelo"),
                            Ano = reader.GetInt32("ano")
                        };
                        avaliacao.ItensPedido.Add(item);
                    }

                    return avaliacao;
                }
            }
        }
    }

    public (int IdPagamento, string FormaPagamento) ObterFormaPagamentoPorPedido(int idPedido)
    {
        using (var connection = new MySqlConnection(_conexaoMySQL))
        {
            connection.Open();

            string sql = "CALL spObterFormaPagamentoPorPedido(@pedido_id)";

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@pedido_id", idPedido);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int idPagamento = reader.GetInt32("id_pagamento");
                        string formaPagamento = reader.GetString("forma_pagamento");
                        return (idPagamento, formaPagamento);
                    }
                    else
                    {
                        throw new Exception("Forma de pagamento não encontrada para o pedido.");
                    }
                }
            }
        }
    }

    public void InserirNotaFiscal(NotaFiscal notaFiscal)
    {
        using (var connection = new MySqlConnection(_conexaoMySQL))
        {
            connection.Open();

            string sql = "INSERT INTO NotaFiscal (id_pagamento, numero_nota, data_emissao, valor_total) VALUES (@id_pagamento, @numero_nota, @data_emissao, @valor_total)";

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@id_pagamento", notaFiscal.IdPagamento);
                command.Parameters.AddWithValue("@numero_nota", notaFiscal.NumeroNota);
                command.Parameters.AddWithValue("@data_emissao", notaFiscal.DataEmissao);
                command.Parameters.AddWithValue("@valor_total", notaFiscal.ValorTotal);

                command.ExecuteNonQuery();
            }
        }
    }

    public NotaFiscal ObterNotaFiscalPorPedido(int idPedido)
    {
        using (var connection = new MySqlConnection(_conexaoMySQL))
        {
            connection.Open();

            
            string sqlNotaFiscal = @"
        SELECT nf.id_nota_fiscal, nf.id_pagamento, nf.numero_nota, nf.data_emissao, nf.valor_total, fp.forma_pagamento 
        FROM NotaFiscal nf
        JOIN FormaPagamento fp ON nf.id_pagamento = fp.id_pedido
        WHERE fp.id_pedido = @pedido_id";

            NotaFiscal notaFiscal = null;

            using (var command = new MySqlCommand(sqlNotaFiscal, connection))
            {
                command.Parameters.AddWithValue("@pedido_id", idPedido);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        notaFiscal = new NotaFiscal
                        {
                            IdNotaFiscal = reader.GetInt32("id_nota_fiscal"),
                            IdPagamento = reader.GetInt32("id_pagamento"),
                            NumeroNota = reader.GetString("numero_nota"),
                            DataEmissao = reader.GetDateTime("data_emissao"),
                            ValorTotal = reader.GetDecimal("valor_total"),
                            FormaPagamento = reader.GetString("forma_pagamento"),
                            Itens = new List<ItensPedido>()
                        };
                    }
                    else
                    {
                        throw new Exception("Nota fiscal não encontrada para o pedido.");
                    }
                }
            }

            // pegar os itens do pedido
            string sqlItensPedido = @"
        SELECT ip.id_item_pedido, ip.id_carro, ip.quantidade, ip.preco_unitario, c.modelo, c.marca, c.ano, c.cor, c.imagem
        FROM itens_pedidos ip
        JOIN carros c ON ip.id_carro = c.id_carro
        WHERE ip.id_pedido = @pedido_id";

            using (var command = new MySqlCommand(sqlItensPedido, connection))
            {
                command.Parameters.AddWithValue("@pedido_id", idPedido);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new ItensPedido
                        {
                            IdItem = reader.GetInt32("id_item_pedido"),
                            IdCarro = reader.GetInt32("id_carro"),
                            Quantidade = reader.GetInt32("quantidade"),
                            PrecoUnitario = reader.GetDecimal("preco_unitario"),
                            Modelo = reader.GetString("modelo"),
                            Marca = reader.GetString("marca"),
                            Ano = reader.GetInt32("ano"),
                            Cor = reader.GetString("cor"),
                            Imagem = reader.GetString("imagem")
                        };
                        notaFiscal.Itens.Add(item);
                    }
                }
            }

            // detalhes do cliente
            string sqlCliente = @"
        SELECT c.id_cliente, c.nome, c.sobrenome, c.cpf_cnpj
FROM clientes c
WHERE c.id_cliente = @codigo_cliente";

            using (var command = new MySqlCommand(sqlCliente, connection))
            {
                command.Parameters.AddWithValue("@codigo_cliente", Cliente.ClienteLogadoId);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        notaFiscal.NomeCliente = reader.GetString("Nome");
                        notaFiscal.SobrenomeCliente = reader.GetString("Sobrenome");
                        notaFiscal.CpfCnpj = reader.GetDecimal("cpf_cnpj");
                    }
                    else
                    {
                        throw new Exception("Cliente não encontrado.");
                    }
                }
            }

            return notaFiscal;
        }
    }





}
