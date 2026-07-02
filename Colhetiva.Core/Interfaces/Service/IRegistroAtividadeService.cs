using Colhetiva.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Colhetiva.Core.Interfaces.Service
{
    public interface IRegistroAtividadeService
    {
        Task<RegistroAtividade> CriarAtividadeAsync(RegistroAtividade novoRegistro);
        Task<List<RegistroAtividade>> GetDiarioPessoalAsync(Guid usuarioId, int ano, int mes);
        Task<List<RegistroAtividade>> GetTimelineHortaAsync(Guid hortaId);
    }
}
