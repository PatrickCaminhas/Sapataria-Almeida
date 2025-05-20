using System.Collections.Generic;
using System.Threading.Tasks;
using Sapataria_Almeida.Models;

namespace Sapataria_Almeida.Repositories
{
    /// <summary>
    /// Interface para acesso às notificações (e outros dados) no repositório.
    /// </summary>
    public interface IRepositorioDados
    {
        Task<List<Notificacao>> ObterNotificacoesNaoLidasAsync();
        Task MarcarNotificacaoComoLidaAsync(int notificacaoId);
    }
}
