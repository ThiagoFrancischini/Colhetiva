using Colhetiva.Core.Entities;
using Colhetiva.Core.Interfaces.Repositories;
using Colhetiva.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;


namespace Colhetiva.Infrastructure.Repositories
{
    public class EstadoRepository : IEstadoRepository
    {
        private readonly ColhetivaDbContext _context;
        public EstadoRepository(ColhetivaDbContext context)
        {
            _context = context;
        }

        public async Task<List<Estado>> GetEstados()
        {
            return await _context
                .Estados
                .OrderBy(x => x.Sigla)
                .ToListAsync();
        }

        public async Task<Estado?> GetById(Guid id)
        {
            return await _context.Estados.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task Salvar(Estado estado)
        {
            var existingState = await _context.Estados.FindAsync(estado.Id);

            if (existingState == null)
            {
                await _context.Estados.AddAsync(estado);
            }
            else
            {
                _context.Entry(existingState).CurrentValues.SetValues(estado);
            }
        }
    }
}
