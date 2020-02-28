using System.Collections.Generic;

namespace RCN.API.Data
{
    public interface IProdutoRepository
    {
         void Inserir(Produto produto);

         void Editar(Produto produto);

         void Excluir(Produto produto);

         Produto Obter(int id);

        IEnumerable<Produto> Obter();
    }
}