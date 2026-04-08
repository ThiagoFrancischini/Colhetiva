using Colhetiva.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colhetiva.Core.Interfaces.Service
{
    public interface IEstadoService
    {
        Task<List<Estado>> GetAllAsync();
        Task<Estado?> GetByIdAsync(Guid id);
        Task<Estado> CriarEstadoAsync(Estado novoEstado);
    }
}
