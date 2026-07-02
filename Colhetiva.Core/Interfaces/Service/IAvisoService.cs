using Colhetiva.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Colhetiva.Core.Interfaces.Service
{
    public interface IAvisoService
    {
        Task<Aviso> PublicarAvisoAsync(Aviso novoAviso);
        Task<List<Aviso>> GetFeedConsolidadoAsync(Guid usuarioId);
    }
}
