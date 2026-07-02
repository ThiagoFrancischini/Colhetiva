using Colhetiva.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Colhetiva.Core.Interfaces.Service
{
    public interface INotificacaoService
    {
        Task<int> GetCountNaoLidasAsync(Guid usuarioId);
        Task<List<Notificacao>> GetNotificacoesAsync(Guid usuarioId);
        Task<Notificacao?> MarcarComoLidaAsync(Guid notificacaoId, Guid usuarioId);
    }
}
