using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sistema_Sapataria.Data;       
using Sistema_Sapataria.Models;

namespace Sistema_Sapataria.Repositories
{
    public class RepositorioDados : IRepositorioDados
    {
        private readonly AppDbContext _ctx;

        public RepositorioDados(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public List<Produto> GetProdutos()
        => _ctx.Produto
                   .OrderBy(p => p.Nome)
                   .ToList();

        public List<ProdutoConserto> GetProdutosConserto()
        => _ctx.ProdutoConserto
                   .OrderBy(p => p.Nome)
                   .ToList();

     

        public async Task<List<Notificacao>> ObterNotificacoesNaoLidasAsync()
            => await _ctx.Set<Notificacao>()
                         .Where(n => !n.Lida)
                         .ToListAsync();

        public async Task MarcarNotificacaoComoLidaAsync(int notificacaoId)
        {
            var n = await _ctx.Set<Notificacao>().FindAsync(notificacaoId);
            if (n != null)
            {
                n.Lida = true;
                await _ctx.SaveChangesAsync();
            }
        }
           public string GetDashboardPassword()
           {
               var admin = _ctx.Administracao.FirstOrDefault();
               return admin?.Senha ?? string.Empty;
           }

            public void SetDashboardPassword(string NovaSenha)
            {
                var login = _ctx.Administracao.FirstOrDefault();
                if (login != null)
                {
                    login.Senha = NovaSenha;
                _ctx.SaveChanges();
                }
            }
        public void AddProduto(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
                return;

            _ctx.Produto.Add(new Models.Produto { Nome = nome.Trim() });
            _ctx.SaveChanges();
        }
        public bool RemoverProduto(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
                return false;

            var produto = _ctx.Produto.FirstOrDefault(p => p.Nome == nome.Trim());
            if (produto != null)
            {
                _ctx.Produto.Remove(produto);
                _ctx.SaveChanges();
                return true;
            }
            return false;
        }


  

        public void AddProdutoConserto(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
                return;

            _ctx.ProdutoConserto.Add(new Models.ProdutoConserto { Nome = nome.Trim() });
            _ctx.SaveChanges();
        }

        public void RemoveProdutoConserto(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
                return;

            _ctx.ProdutoConserto.Add(new Models.ProdutoConserto { Nome = nome.Trim() });
            _ctx.SaveChanges();
        }


    }
}
