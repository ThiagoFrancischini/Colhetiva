using Colhetiva.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Colhetiva.Core.Interfaces.Repositories
{
    public interface ISolicitacaoRepository
    {
        Task<List<Solicitacao>> GetSolicitacoes();
        Task Salvar(Solicitacao solicitacao);
        Task<Solicitacao?> GetById(Guid id);
    }
}