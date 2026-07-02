using Colhetiva.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Colhetiva.Core.Interfaces.Repositories
{
    public interface IRegistroAtividadeRepository
    {
        Task Salvar(RegistroAtividade registro);
        Task<RegistroAtividade?> GetById(Guid id);
        Task<List<RegistroAtividade>> GetDiarioPessoal(Guid usuarioId, DateTime inicioUtc, DateTime fimUtc);
        Task<List<RegistroAtividade>> GetTimelineHorta(Guid hortaId, int take = 200);
    }
}
