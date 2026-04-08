using Colhetiva.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colhetiva.Core.Interfaces.Repositories
{
    public interface ICidadeRepository
    {
        Task<List<Cidade>> GetCidadesPorEstado(Guid estadoId);
        Task<Cidade?> GetById(Guid id);
        Task Salvar(Cidade cidade);
    }
}
