using Colhetiva.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Colhetiva.Core.Interfaces.Service
{
    public interface IHortaService
    {
        Task<List<Horta>> GetAllAsync();
        Task<Horta?> GetByIdAsync(Guid id);
        Task<Horta> CriarHortaAsync(Horta novaHorta);
    }
}