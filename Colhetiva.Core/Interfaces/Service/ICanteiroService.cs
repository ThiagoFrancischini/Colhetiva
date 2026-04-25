using Colhetiva.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Colhetiva.Core.Interfaces.Service
{
    public interface ICanteiroService
    {
        Task<List<Canteiro>> GetAllAsync();
        Task<Canteiro?> GetByIdAsync(Guid id);
        Task<Canteiro> CriarCanteiroAsync(Canteiro novoCanteiro);
    }
}