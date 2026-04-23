using Colhetiva.Core.Entities;
using Colhetiva.Core.Interfaces.Repositories;
using Colhetiva.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Colhetiva.Infrastructure.Repositories
{
    public class FerramentaRepository : IFerramentaRepository
    {
        private readonly ColhetivaDbContext _context;
        public FerramentaRepository(ColhetivaDbContext context)
        {
            _context = context;
        }

        public async Task<List<Ferramenta>> GetFerramentas()
        {
            return await _context
                .Ferramentas
                .Include(f => f.Horta)
                .OrderBy(f => f.Nome)
                .ToListAsync();
        }

        public async Task<Ferramenta?> GetById(Guid id)
        {
            return await _context.Ferramentas
                .AsNoTracking()
                .Include(f => f.Horta)
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task Salvar(Ferramenta ferramenta)
        {
            var existingFerramenta = await _context.Ferramentas.FindAsync(ferramenta.Id);

            if (existingFerramenta == null)
            {
                await _context.Ferramentas.AddAsync(ferramenta);
            }
            else
            {
                _context.Entry(existingFerramenta).CurrentValues.SetValues(ferramenta);
            }
        }
    }
}