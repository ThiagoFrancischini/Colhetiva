using Colhetiva.Core.Entities;
using Colhetiva.Core.Interfaces.Repositories;
using Colhetiva.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colhetiva.Infrastructure.Repositories
{
    public class UserContextRepository : IUserContextRepository
    {
        private readonly ColhetivaDbContext _context;

        public UserContextRepository(ColhetivaDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserContext>> GetByUsuarioIdAsync(Guid usuarioId)
        {
            return await _context.Set<UserContext>()
                .Where(uc => uc.UsuarioId == usuarioId)
                .ToListAsync();
        }

        public async Task AddAsync(UserContext userContext)
        {
            await _context.Set<UserContext>().AddAsync(userContext);
        }

        public async Task UpdateAsync(UserContext userContext)
        {
            _context.Set<UserContext>().Update(userContext);
            await Task.CompletedTask;
        }

        public async Task RemoveAsync(Guid id)
        {
            var userContext = await _context.Set<UserContext>().FindAsync(id);
            if (userContext != null)
            {
                _context.Set<UserContext>().Remove(userContext);
            }
        }
    }
}
