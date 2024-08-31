using overhaul_teste.Models;
using MySqlX.XDevAPI;
using Newtonsoft.Json;
/*instala o pacote newtosoft.json */
/*contém classes que fornecem serviços e objetos que você precisa para escrever suas aplicações. Além disso, 
o Framework Class Library possui uma hierarquia de classes que fornecem funcionalidades para os mais diversos tipos de necessidades do usuário*/
namespace overhaul_teste.Libraries.Login
{
    public class LoginCliente
    {

        //injeção de dependencia
        private string Key = "Login.Cliente";
        private Sessao.Sessao _sessao;

        //Construtor
        public LoginCliente(Sessao.Sessao sessao)
        {
            _sessao = sessao;
        }

        public void Login(Cliente cliente) //método de logar
        {
            // Serializar- Com a serialização é possível salvar objetos em arquivos de dados
            string clienteJSONString = JsonConvert.SerializeObject(cliente);
        }

        public Cliente GetCliente()
        {
            /* Deserializar-Já a desserialização permite que os 
            objetos persistidos em arquivos possam ser recuperados e seus valores recriados na memória*/

            if (_sessao.Existe(Key))
            {
                string clienteJSONString = _sessao.Consultar(Key);
                return JsonConvert.DeserializeObject<Cliente>(clienteJSONString);
            }
            else
            {
                return null;
            }
        }

        public void Logout() //saindo do logado e fechando a sessao
        {
            _sessao.RemoverTodos();
        }

    }
}
