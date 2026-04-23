using Colhetiva.Core.Entities;
using Colhetiva.Core.Interfaces.Repositories;
using Colhetiva.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Colhetiva.Infrastructure.Repositories
{
    public class CanteiroRepository : ICanteiroRepository
    {
        private readonly ColhetivaDbContext _context;
        public CanteiroRepository(ColhetivaDbContext context)
        {
            _context = context;
        }

        public async Task<List<Canteiro>> GetCanteiros()
        {
            return await _context
                .Canteiros
                .Include(c => c.Horta)
                .ThenInclude(h => h.Endereco)
                .OrderBy(c => c.Identificacao)
                .ToListAsync();
        }

        public async Task<Canteiro?> GetById(Guid id)
        {
            return await _context.Canteiros
                .AsNoTracking()
                .Include(c => c.Horta)
                .ThenInclude(h => h.Endereco)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task Salvar(Canteiro canteiro)
        {
            var existingCanteiro = await _context.Canteiros.FindAsync(canteiro.Id);

            if (existingCanteiro == null)
            {
                await _context.Canteiros.AddAsync(canteiro);
            }
            else
            {
                _context.Entry(existingCanteiro).CurrentValues.SetValues(canteiro);
            }
        }
    }
}