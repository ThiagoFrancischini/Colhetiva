using Colhetiva.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Colhetiva.Core.Interfaces.Repositories
{
    public interface IFerramentaRepository
    {
        Task<List<Ferramenta>> GetFerramentas();
        Task Salvar(Ferramenta ferramenta);
        Task<Ferramenta?> GetById(Guid id);
    }
}