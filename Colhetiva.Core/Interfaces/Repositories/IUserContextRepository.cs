using Colhetiva.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Colhetiva.Core.Interfaces.Repositories
{
    public interface IUserContextRepository
    {
        Task<IEnumerable<UserContext>> GetByUsuarioIdAsync(Guid usuarioId);
        Task AddAsync(UserContext userContext);
        Task UpdateAsync(UserContext userContext);
        Task RemoveAsync(Guid id);
    }
}
