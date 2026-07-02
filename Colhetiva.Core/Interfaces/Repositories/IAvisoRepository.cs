using Colhetiva.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Colhetiva.Core.Interfaces.Repositories
{
    public interface IAvisoRepository
    {
        Task Salvar(Aviso aviso);
        Task<Aviso?> GetById(Guid id);
        Task<List<Aviso>> GetFeedPorUsuarioAsync(Guid usuarioId);
    }
}
