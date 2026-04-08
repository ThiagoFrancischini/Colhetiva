using Colhetiva.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colhetiva.Core.Interfaces.Repositories
{
    public interface IEstadoRepository
    {
        Task<List<Estado>> GetEstados();
        Task Salvar(Estado estado);
        Task<Estado?> GetById(Guid id);
    }
}
