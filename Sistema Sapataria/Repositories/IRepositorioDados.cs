using System.Collections.Generic;
using System.Threading.Tasks;
using Sistema_Sapataria.Models;

namespace Sistema_Sapataria.Repositories
{
    /// <summary>
    /// Interface para acesso às notificações (e outros dados) no repositório.
    /// </summary>
    public interface IRepositorioDados
    {
        Task<List<Notificacao>> ObterNotificacoesNaoLidasAsync();
        Task MarcarNotificacaoComoLidaAsync(int notificacaoId);
        void AddProduto(string nome);
        void AddProdutoConserto(string nome);
        public bool RemoverProduto(string nome);
        public List<Produto> GetProdutos();
        public List<ProdutoConserto> GetProdutosConserto();

    }
}
