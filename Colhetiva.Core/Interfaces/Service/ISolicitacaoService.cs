using Colhetiva.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Colhetiva.Core.Interfaces.Service
{
    public interface ISolicitacaoService
    {
        Task<List<Solicitacao>> GetAllAsync();
        Task<Solicitacao?> GetByIdAsync(Guid id);
        Task<Solicitacao> CriarSolicitacaoAsync(Solicitacao novaSolicitacao);
    }
}