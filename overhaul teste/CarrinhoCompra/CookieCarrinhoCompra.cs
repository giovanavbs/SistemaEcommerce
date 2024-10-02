using Newtonsoft.Json;
using overhaul_teste.Models;
using System.Linq;

namespace overhaul_teste.CarrinhoCompra
{
    public class CookieCarrinhoCompra
    {
        private string Key = "Carrinho.Compras";
        private Cookie.Cookie _cookie;

        public CookieCarrinhoCompra(Cookie.Cookie cookie)
        {
            _cookie = cookie;
        }

        // Salvar lista de itens do carrinho no cookie
        public void Salvar(List<Carrinho> lista)
        {
            string valor = JsonConvert.SerializeObject(lista);
            _cookie.Cadastrar(Key, valor);
        }

        // Consultar itens do carrinho a partir do cookie
        public List<Carrinho> Consultar()
        {
            if (_cookie.Existe(Key))
            {
                string valor = _cookie.Consultar(Key);
                return JsonConvert.DeserializeObject<List<Carrinho>>(valor);
            }
            else
            {
                return new List<Carrinho>();
            }
        }

        // Cadastrar um item novo ou adicionar quantidade no carrinho
        public void Cadastrar(Carrinho item)
        {
            List<Carrinho> lista;
            if (_cookie.Existe(Key))
            {
                lista = Consultar();
                var itemLocalizado = lista.SingleOrDefault(a => a.IdCarro == item.IdCarro);

                if (itemLocalizado == null)
                {
                    item.QuantidadeProd = 1;
                    lista.Add(item);
                }
                else
                {
                    itemLocalizado.QuantidadeProd += 1;
                }
            }
            else
            {
                lista = new List<Carrinho> { item };
                item.QuantidadeProd = 1;
            }

            Salvar(lista);
        }

        // Atualizar a quantidade de um item no carrinho
        public void Atualizar(Carrinho item)
        {
            var lista = Consultar();
            var itemLocalizado = lista.SingleOrDefault(a => a.IdCarro == item.IdCarro);

            if (itemLocalizado != null)
            {
                itemLocalizado.QuantidadeProd = item.QuantidadeProd;
                Salvar(lista);
            }
        }

        // Remover um item do carrinho
        public void Remover(Carrinho item)
        {
            var lista = Consultar();
            var itemLocalizado = lista.SingleOrDefault(a => a.IdCarro == item.IdCarro);

            if (itemLocalizado != null)
            {
                lista.Remove(itemLocalizado);
                Salvar(lista);
            }
        }

        // Diminuir a quantidade de um produto no carrinho
        public void DiminuirProduto(Carrinho item)
        {
            var lista = Consultar();
            var itemLocalizado = lista.SingleOrDefault(a => a.IdCarro == item.IdCarro);

            if (itemLocalizado != null && itemLocalizado.QuantidadeProd > 1)
            {
                itemLocalizado.QuantidadeProd -= 1;
                Salvar(lista);
            }
        }

        // Remover todos os itens do carrinho
        public void RemoverTodos()
        {
            _cookie.Remover(Key);
        }

        // Verificar se o carrinho existe no cookie
        public bool Existe(string key)
        {
            return _cookie.Existe(key);
        }
    }
}
