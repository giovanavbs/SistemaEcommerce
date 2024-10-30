using MySql.Data.MySqlClient;
using overhaul_teste.Models;
using overhaul_teste.ViewModels;
using System.Data;
namespace overhaul_teste.Repositorio
{
    public class ClienteRepositorio : IClienteRepositorio
    {

        private readonly string? _conexaoMySQL;

        public ClienteRepositorio(IConfiguration conf) => _conexaoMySQL = conf.GetConnectionString("ConexaoMySQL");

        // metodo login cliente

        /*public Cliente Login(string Email, string Senha)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();

                MySqlCommand cmd = new MySqlCommand("SELECT clientes.id_cliente, clientes.email, clientes.senha FROM login INNER JOIN clientes ON login.id_cliente = clientes.id_cliente WHERE login.email = @Email AND login.senha = @Senha", conexao);

                cmd.Parameters.Add("@Email", MySqlDbType.VarChar).Value = Email;
                cmd.Parameters.Add("@Senha", MySqlDbType.VarChar).Value = Senha;

                MySqlDataReader dr;
                Cliente cliente = new Cliente();

                dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                if (dr.Read())
                {
                    cliente.Codigo = Convert.ToInt32(dr["id_cliente"]);
                    cliente.Email = Convert.ToString(dr["email"]);
                    cliente.Senha = Convert.ToString(dr["senha"]);
                }

                return cliente;
            }
        } */

        public Cliente Login(string Email, string Senha)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();

                MySqlCommand cmd = new MySqlCommand(
                    "SELECT clientes.id_cliente, clientes.email, clientes.senha, clientes.nivel_acesso FROM login " + 
                    "INNER JOIN clientes ON login.id_cliente = clientes.id_cliente " +
                    "WHERE login.email = @Email AND login.senha = @Senha", conexao);

                cmd.Parameters.Add("@Email", MySqlDbType.VarChar).Value = Email;
                cmd.Parameters.Add("@Senha", MySqlDbType.VarChar).Value = Senha;

                MySqlDataReader dr;
                Cliente cliente = new Cliente();

                dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                if (dr.Read())
                {
                    // guardar na classe apos o login
                    cliente.Codigo = Convert.ToInt32(dr["id_cliente"]);
                    cliente.Email = Convert.ToString(dr["email"]);
                    cliente.Senha = Convert.ToString(dr["senha"]);

                    // globais
                    Cliente.ClienteLogadoId = cliente.Codigo;
                    Cliente.NivelAcesso = Convert.ToInt32(dr["nivel_acesso"]); 

                    ObterClientePorEmail(cliente.Email, cliente);
                }

                return cliente;
            }
        }


        public void ObterClientePorEmail(string email, Cliente cliente)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();

                MySqlCommand cmd = new MySqlCommand("spObterClientePorEmail", conexao);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@vEmail", MySqlDbType.VarChar).Value = email;

                MySqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    cliente.Nome = Convert.ToString(dr["nome"]);
                    cliente.Sobrenome = Convert.ToString(dr["sobrenome"]);
                    cliente.Telefone = dr["telefone"] != DBNull.Value ? Convert.ToInt32(dr["telefone"]) : (int?)null;
                    cliente.TipoCliente = Convert.ToString(dr["tipo_cliente"]);
                    cliente.CepCli = Convert.ToInt32(dr["CepCli"]);
                    cliente.NumEnd = Convert.ToInt32(dr["NumEnd"]);
                    cliente.CompEnd = Convert.ToString(dr["CompEnd"]);
                    cliente.Logradouro = Convert.ToString(dr["Logradouro"]);
                    cliente.Bairro = Convert.ToString(dr["Bairro"]);
                    cliente.Cidade = Convert.ToString(dr["Cidade"]);
                    cliente.UF = Convert.ToString(dr["UF"]);
                    cliente.DataCadastro = Convert.ToDateTime(dr["data_cadastro"]);
                    cliente.CpfCnpj = Convert.ToDecimal(dr["cpf_cnpj"]);
                }
            }
        }


        // metodo cadastrar cliente
        public void Cadastrar(Cliente cliente)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))

            {
                conexao.Open();

                MySqlCommand cmd = new MySqlCommand("spinsertCliente", conexao)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };

                cmd.Parameters.Add("@vNome", MySqlDbType.VarChar).Value = cliente.Nome;
                cmd.Parameters.Add("@vSobrenome", MySqlDbType.VarChar).Value = cliente.Sobrenome;
                cmd.Parameters.Add("@vEmail", MySqlDbType.VarChar).Value = cliente.Email;
                cmd.Parameters.Add("@vSenha", MySqlDbType.VarChar).Value = cliente.Senha;
                cmd.Parameters.Add("@vTelefone", MySqlDbType.Decimal).Value = cliente.Telefone;
                cmd.Parameters.Add("@vTipoCliente", MySqlDbType.Enum).Value = cliente.TipoCliente;
                cmd.Parameters.Add("@vCepCli", MySqlDbType.Int32).Value = cliente.CepCli;
                cmd.Parameters.Add("@vNumEnd", MySqlDbType.Int32).Value = cliente.NumEnd;
                cmd.Parameters.Add("@vCompEnd", MySqlDbType.VarChar).Value = cliente.CompEnd;
                cmd.Parameters.Add("@vLogradouro", MySqlDbType.VarChar).Value = cliente.Logradouro;
                cmd.Parameters.Add("@vBairro", MySqlDbType.VarChar).Value = cliente.Bairro;
                cmd.Parameters.Add("@vCidade", MySqlDbType.VarChar).Value = cliente.Cidade;
                cmd.Parameters.Add("@vUF", MySqlDbType.VarChar).Value = cliente.UF;
                cmd.Parameters.Add("@vCpfCnpj", MySqlDbType.Decimal).Value = cliente.CpfCnpj;


                cmd.ExecuteNonQuery();
                conexao.Close();
            }

        }

        // alterar cliente
        public void Atualizar(Cliente cliente)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand("spUpdateCliente", conexao)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.Add("@vIdCliente", MySqlDbType.Int32).Value = cliente.Codigo;
                cmd.Parameters.Add("@vSobrenome", MySqlDbType.VarChar).Value = cliente.Sobrenome;
                cmd.Parameters.Add("@vEmail", MySqlDbType.VarChar).Value = cliente.Email;
                cmd.Parameters.Add("@vSenha", MySqlDbType.VarChar).Value = cliente.Senha;
                cmd.Parameters.Add("@vTelefone", MySqlDbType.Decimal).Value = cliente.Telefone;
                cmd.Parameters.Add("@vTipoCliente", MySqlDbType.Enum).Value = cliente.TipoCliente;
                cmd.Parameters.Add("@vCepCli", MySqlDbType.Int32).Value = cliente.CepCli;
                cmd.Parameters.Add("@vNumEnd", MySqlDbType.Int32).Value = cliente.NumEnd;
                cmd.Parameters.Add("@vCompEnd", MySqlDbType.VarChar).Value = cliente.CompEnd;
                cmd.Parameters.Add("@vLogradouro", MySqlDbType.VarChar).Value = cliente.Logradouro;
                cmd.Parameters.Add("@vBairro", MySqlDbType.VarChar).Value = cliente.Bairro;
                cmd.Parameters.Add("@vCidade", MySqlDbType.VarChar).Value = cliente.Cidade;
                cmd.Parameters.Add("@vUF", MySqlDbType.VarChar).Value = cliente.UF;
                cmd.Parameters.Add("@vCpfCnpj", MySqlDbType.Decimal).Value = cliente.CpfCnpj;

                cmd.ExecuteNonQuery();
                conexao.Close();
            }
        }

        // obter um cliente pelo id
        public Cliente ObterCliente(int id)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand("spObterClienteID", conexao)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@vIdCliente", id);

                MySqlDataReader dr = cmd.ExecuteReader();

                Cliente cliente = new Cliente();

                while (dr.Read())
                {
                    cliente.Codigo = Convert.ToInt32(dr["Codigo"]);
                    cliente.Nome = dr["nome"].ToString();
                    cliente.Sobrenome = dr["sobrenome"].ToString();
                    cliente.Email = dr["email"].ToString();
                    cliente.Telefone = dr["telefone"] != DBNull.Value ? Convert.ToInt32(dr["telefone"]) : (int?)null;
                    cliente.TipoCliente = dr["tipo_cliente"].ToString();
                    cliente.CepCli = Convert.ToInt32(dr["CepCli"]);
                    cliente.NumEnd = Convert.ToInt32(dr["NumEnd"]);
                    cliente.CompEnd = dr["CompEnd"].ToString();
                    cliente.Logradouro = dr["Logradouro"].ToString();
                    cliente.Bairro = dr["Bairro"].ToString();
                    cliente.Cidade = dr["Cidade"].ToString();
                    cliente.UF = dr["UF"].ToString();
                    cliente.DataCadastro = Convert.ToDateTime(dr["data_cadastro"]);
                    cliente.CpfCnpj = Convert.ToDecimal(dr["cpf_cnpj"]);

                }

                conexao.Close();
                return cliente;
            }
        }

        // excluir cliente
        public void Excluir(int Id)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand("spExcluirCliente", conexao)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@vIdCliente", Id);
                cmd.ExecuteNonQuery();
                conexao.Close();
            }

            Cliente.ClienteLogadoId = 0;
            Cliente.NivelAcesso = 0;

            // zerar a instancia do cliente ao excluir pra deslogar
            var cliente = new Cliente
            {
                Codigo = 0,
                Nome = null,
                Sobrenome = null,
                Telefone = null,
                Email = null,
                Senha = null,
                TipoCliente = null,
                CepCli = 0,
                NumEnd = 0,
                CompEnd = null,
                Logradouro = null,
                Bairro = null,
                Cidade = null,
                UF = null,
                DataCadastro = DateTime.MinValue,
                CpfCnpj = 0
            };
    }

        public void InserirCartao(Cartao cartao, int idCliente)
        {
            using (var connection = new MySqlConnection(_conexaoMySQL))
            {
                connection.Open();
                using (var command = new MySqlCommand("spInsertCartao", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("p_id_cliente", idCliente);
                    command.Parameters.AddWithValue("p_numero_cartao", cartao.NumeroCartao);
                    command.Parameters.AddWithValue("p_nome_titular", cartao.NomeTitular);
                    command.Parameters.AddWithValue("p_validade", cartao.Validade);
                    command.Parameters.AddWithValue("p_cvv", cartao.CVV);
                    command.Parameters.AddWithValue("p_bandeira", cartao.Bandeira);

                    command.ExecuteNonQuery();
                }
            }
        }

        public List<Cartao> ObterCartoes(int idCliente)
        {
            var cartoes = new List<Cartao>();

            using (var connection = new MySqlConnection(_conexaoMySQL))
            {
                connection.Open();

                string query = "SELECT id_cartao, numero_cartao, nome_titular, validade, cvv, bandeira FROM cartoes WHERE id_cliente = @idCliente";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@idCliente", idCliente);

                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            Console.WriteLine("Nenhum cartão encontrado."); 
                            return cartoes;
                        }

                        while (reader.Read())
                        {
                            try
                            {
                                var cartao = new Cartao
                                {
                                    IdCartao = reader.GetInt32("id_cartao"),
                                    NumeroCartao = reader.GetString("numero_cartao"),
                                    NomeTitular = reader.GetString("nome_titular"),
                                    Validade = reader.GetString("validade"), 
                                    CVV = reader.GetString("cvv"),
                                    Bandeira = reader.GetString("bandeira")
                                };

                                cartoes.Add(cartao);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Erro ao ler cartão: {ex.Message}");
                            }
                        }
                    }
                }
            }

            return cartoes;
        }


        public void InserirEndereco(Cliente cliente, int idPedido, int idCliente)
        {
            using (var connection = new MySqlConnection(_conexaoMySQL))
            {
                connection.Open();

                // inserir na tb endereço se nao tiver ainda
                using (var command = new MySqlCommand("spinsertendereco", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("vCEP", cliente.CepCli);
                    command.Parameters.AddWithValue("vLogradouro", cliente.Logradouro);
                    command.Parameters.AddWithValue("vBairro", cliente.Bairro);
                    command.Parameters.AddWithValue("vCidade", cliente.Cidade);
                    command.Parameters.AddWithValue("vUF", cliente.UF);
                    command.ExecuteNonQuery();
                }

                // inserir na tb entrega
                using (var commandEntrega = new MySqlCommand("spinsertentrega", connection))
                {
                    commandEntrega.CommandType = CommandType.StoredProcedure;
                    commandEntrega.Parameters.AddWithValue("vIdPedido", idPedido);
                    commandEntrega.Parameters.AddWithValue("vIdCliente", idCliente);
                    commandEntrega.Parameters.AddWithValue("vCEP", cliente.CepCli);
                    commandEntrega.Parameters.AddWithValue("vLogradouro", cliente.Logradouro);
                    commandEntrega.Parameters.AddWithValue("vBairro", cliente.Bairro);
                    commandEntrega.Parameters.AddWithValue("vCidade", cliente.Cidade);
                    commandEntrega.Parameters.AddWithValue("vUF", cliente.UF);
                    commandEntrega.ExecuteNonQuery();
                }
            }
        }

        public void InserirEnderecoRetirada(int idPedido, int idCliente)
        {
            using (var connection = new MySqlConnection(_conexaoMySQL))
            {
                connection.Open();

                using (var command = new MySqlCommand("InserirEnderecoRetirada", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("vIdPedido", idPedido);
                    command.Parameters.AddWithValue("vIdCliente", idCliente);

                    command.ExecuteNonQuery();
                }
            }
        }

        public void InserirEnderecoAtualNaEntrega(Cliente cliente, int idPedido, int idCliente)
        {
            using (var connection = new MySqlConnection(_conexaoMySQL))
            {
                connection.Open();

                using (var commandEntrega = new MySqlCommand("spinsertentrega", connection))
                {
                    commandEntrega.CommandType = CommandType.StoredProcedure;
                    commandEntrega.Parameters.AddWithValue("vIdPedido", idPedido);
                    commandEntrega.Parameters.AddWithValue("vIdCliente", idCliente);
                    commandEntrega.Parameters.AddWithValue("vCEP", cliente.CepCli);
                    commandEntrega.Parameters.AddWithValue("vLogradouro", cliente.Logradouro);
                    commandEntrega.Parameters.AddWithValue("vBairro", cliente.Bairro);
                    commandEntrega.Parameters.AddWithValue("vCidade", cliente.Cidade);
                    commandEntrega.Parameters.AddWithValue("vUF", cliente.UF);
                    commandEntrega.ExecuteNonQuery();
                }
            }
        }

        public List<Pedido> VerPedidosCliente(int idCliente)
        {
            var pedidos = new List<Pedido>();

            using (var connection = new MySqlConnection(_conexaoMySQL))
            {
                connection.Open();

                using (var command = new MySqlCommand("spObterPedidosEItensCliente", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@cliente_id", idCliente);

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
                                Itens = new List<ItensPedido>()
                            };

                            if (!reader.IsDBNull(reader.GetOrdinal("id_item_pedido")))
                            {
                                var item = new ItensPedido
                                {
                                    IdItem = reader.GetInt32("id_item_pedido"),
                                    Modelo = reader.GetString("modelo_carro"),
                                    Marca = reader.GetString("marca_carro"),
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

        public PagamentoConfirmadoViewModel ObterDetalhesPedido(int idPedido)
        {
            var viewModel = new PagamentoConfirmadoViewModel();

            using (var connection = new MySqlConnection(_conexaoMySQL))
            {
                connection.Open();

                using (var command = new MySqlCommand("spObterDetalhesPedidoPorID", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@p_idPedido", idPedido);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {

                            viewModel.Pedido = new Pedido
                            {
                                IdPedido = reader.GetInt32("id_pedido"),
                                DataPedido = reader.GetDateTime("data_pedido"),
                                ValorTotal = reader.GetDecimal("valor_total"),
                                StatusPedido = reader.GetString("status_pedido"),
                            };


                            do
                            {
                                if (!reader.IsDBNull(reader.GetOrdinal("id_item_pedido")))
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
                                    viewModel.Itens.Add(item);
                                }

                                if (reader.FieldCount > 10) 
                                {
                                    viewModel.EnderecoEntrega = new Endereco
                                    {
                                        Logradouro = reader.GetString("logradouro"),
                                        CEP = reader.GetInt32("cep"),
                                        BairroNome = reader.GetString("BairroNome"),
                                        CidadeNome = reader.GetString("CidadeNome"),
                                        UFNome = reader.GetString("UFNome")
                                    };
                                }
                            } while (reader.Read());
                        }
                    }
                }
            }

            return viewModel;
        }

    }


}

