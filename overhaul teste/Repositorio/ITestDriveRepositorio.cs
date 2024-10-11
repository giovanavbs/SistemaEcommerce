using overhaul_teste.Models;

namespace overhaul_teste.Repositorio
{
    public interface ITestDriveRepositorio
    {
        void InserirTestDrive(int id_cliente, int id_carro, DateTime data_test);

        TestDrive ObterTestDrive(int idCliente, int idTest);

        int ObterTestDriveRecenteID(int idCliente);
    }
}
