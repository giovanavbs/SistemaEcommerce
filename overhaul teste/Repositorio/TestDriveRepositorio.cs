using MySql.Data.MySqlClient;
using overhaul_teste.Models;
using System.Data;
using System.Data.Common;

namespace overhaul_teste.Repositorio
{
    public class TestDriveRepositorio : ITestDriveRepositorio
    {
        private readonly string? _conexaoMySQL;

        public TestDriveRepositorio(IConfiguration conf) => _conexaoMySQL = conf.GetConnectionString("ConexaoMySQL");

        public void InserirTestDrive(int idCliente, int idCarro, DateTime dataTest)
        {
            TestDrive testDrive = new TestDrive
            {
                IdCliente = idCliente,
                IdCarro = idCarro,
                DataTest = dataTest
            };

            using (var connection = new MySqlConnection(_conexaoMySQL))
            {
                connection.Open();
                using (var command = new MySqlCommand("INSERT INTO test_drive (id_cliente, id_carro, data_test) VALUES (@idCliente, @idCarro, @dataTest)", connection))
                {
                    command.Parameters.AddWithValue("@idCliente", testDrive.IdCliente);
                    command.Parameters.AddWithValue("@idCarro", testDrive.IdCarro);
                    command.Parameters.AddWithValue("@dataTest", testDrive.DataTest);
                    command.ExecuteNonQuery();
                }
            }
        }


        public TestDrive ObterTestDrive(int idCliente, int idTest)
        {
            TestDrive testDrive = null;

            using (var connection = new MySqlConnection(_conexaoMySQL))
            {
                connection.Open();

                using (var command = new MySqlCommand("spExibirTestDrive", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("p_id_cliente", idCliente);
                    command.Parameters.AddWithValue("p_id_test", idTest);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            testDrive = new TestDrive
                            {
                                IdTest = reader.GetInt32("id_test"),
                                IdCliente = reader.GetInt32("id_cliente"),
                                IdCarro = reader.GetInt32("id_carro"),
                                DataTest = reader.GetDateTime("data_test"),
                                ModeloCarro = reader.GetString("modelo_carro"),
                                MarcaCarro = reader.GetString("marca_carro"),
                                CorCarro = reader.GetString("cor_carro"),
                                PrecoCarro = reader.GetDecimal("preco_carro")
                            };
                        }
                    }
                }
            }

            return testDrive;
        }


        public int ObterTestDriveRecenteID(int idCliente)
        {
            int idTest = 0;

            using (var connection = new MySqlConnection(_conexaoMySQL))
            {
                connection.Open();
                string sql = "SELECT id_test FROM test_drive WHERE id_cliente = @id_cliente ORDER BY id_test DESC LIMIT 1";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id_cliente", idCliente);

                    var result = command.ExecuteScalar();
                    if (result != null)
                    {
                        idTest = Convert.ToInt32(result);
                    }
                }
            }

            return idTest;
        }


    }
}
