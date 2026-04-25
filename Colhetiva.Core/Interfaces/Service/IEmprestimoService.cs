using Colhetiva.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Colhetiva.Core.Interfaces.Service
{
    public interface IEmprestimoService
    {
        Task<List<Emprestimo>> GetAllAsync();
        Task<Emprestimo?> GetByIdAsync(Guid id);
        Task<Emprestimo> CriarEmprestimoAsync(Emprestimo novoEmprestimo);
    }
}