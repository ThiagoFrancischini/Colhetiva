using Colhetiva.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Colhetiva.Core.Interfaces.Repositories
{
    public interface INotificacaoRepository
    {
        Task Salvar(Notificacao notificacao);
        Task<Notificacao?> GetByIdAsync(Guid id);
        Task<List<Notificacao>> GetPorUsuarioAsync(Guid usuarioId);
        Task<int> CountNaoLidasAsync(Guid usuarioId);
    }
}
