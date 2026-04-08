using Colhetiva.Core.Entities;
using Colhetiva.Core.Interfaces.Repositories;
using Colhetiva.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Colhetiva.Infrastructure.Repositories
{
    public class CidadeRepository : ICidadeRepository
    {
        private readonly ColhetivaDbContext _context;

        public CidadeRepository(ColhetivaDbContext context)
        {
            _context = context;
        }

        public async Task<Cidade?> GetById(Guid id)
        {
            return await _context.Cidades
                .Include(c => c.Estado)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<List<Cidade>> GetCidadesPorEstado(Guid estadoId)
        {
            return await _context.Cidades
                .Where(c => c.EstadoId == estadoId)
                .OrderBy(c => c.Nome)
                .ToListAsync();
        }

        public async Task Salvar(Cidade cidade)
        {
            var existingCity = await _context.Cidades.FindAsync(cidade.Id);

            if (existingCity == null)
            {
                await _context.Cidades.AddAsync(cidade);
            }
            else
            {
                _context.Entry(existingCity).CurrentValues.SetValues(cidade);
            }
        }
    }
}
