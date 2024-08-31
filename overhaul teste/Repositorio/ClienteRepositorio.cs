using MySql.Data.MySqlClient;
using overhaul_teste.Models;
using System.Data; // sistema entender que vamos manipular dados
namespace overhaul_teste.Repositorio
{
    // chamar a interface com herança
    public class ClienteRepositorio : IClienteRepositorio
    {
        //declarando a varival de da string de conxão

        private readonly string? _conexaoMySQL;

        //metodo da conexão com banco de dados
        public ClienteRepositorio(IConfiguration conf) => _conexaoMySQL = conf.GetConnectionString("ConexaoMySQL");

        //Login Cliente(metodo )

        public Cliente Login(string Email, string Senha)
        {
            //usando a variavel conexao 
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                //abre a conexão com o banco de dados
                conexao.Open();

                // variavel cmd que receb o select do banco de dados buscando email e senha
                MySqlCommand cmd = new MySqlCommand("select * from login where email = @Email and senha = @Senha", conexao);

                //os paramentros do email e da senha 
                cmd.Parameters.Add("@Email", MySqlDbType.VarChar).Value = Email; // @email vem do banco e o azul vem do visual studio
                cmd.Parameters.Add("@Senha", MySqlDbType.VarChar).Value = Senha;

                //Le os dados que foi pego do email e senha do banco de dados
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                //guarda os dados que foi pego do email e senha do banco de dados
                MySqlDataReader dr;

                //instanciando a model cliente
                Cliente cliente = new Cliente();
                //executando os comandos do mysql e passsando paa a variavel dr
                dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                //verifica todos os dados que foram pego do banco e pega o email e senha
                while (dr.Read())
                {

                    cliente.Email = Convert.ToString(dr["email"]);
                    cliente.Senha = Convert.ToString(dr["senha"]);
                }
                return cliente;
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


                cmd.ExecuteNonQuery();
                conexao.Close();
            }

        }
    }
}
