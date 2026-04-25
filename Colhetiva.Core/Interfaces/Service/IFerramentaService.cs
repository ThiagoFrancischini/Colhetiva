using Colhetiva.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Colhetiva.Core.Interfaces.Service
{
    public interface IFerramentaService
    {
        Task<List<Ferramenta>> GetAllAsync();
        Task<Ferramenta?> GetByIdAsync(Guid id);
        Task<Ferramenta> CriarFerramentaAsync(Ferramenta novaFerramenta);
    }
}