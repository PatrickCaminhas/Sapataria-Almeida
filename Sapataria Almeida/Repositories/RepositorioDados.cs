using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sapataria_Almeida.Data;       
using Sapataria_Almeida.Models;

namespace Sapataria_Almeida.Repositories
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
               // Supondo que você tenha uma tabela Configuracoes com uma coluna DashboardPassword
               var cfg = _ctx.Administracao.FirstOrDefault();
               return cfg?.Senha ?? string.Empty;
           }
    }
}
