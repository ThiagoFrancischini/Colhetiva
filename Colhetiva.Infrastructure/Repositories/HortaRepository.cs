using Colhetiva.Core.Entities;
using Colhetiva.Core.Interfaces.Repositories;
using Colhetiva.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Colhetiva.Infrastructure.Repositories
{
    public class HortaRepository : IHortaRepository
    {
        private readonly ColhetivaDbContext _context;
        public HortaRepository(ColhetivaDbContext context)
        {
            _context = context;
        }

        public async Task<List<Horta>> GetHortas()
        {
            return await _context
                .Hortas
                .Include(h => h.Endereco)
                .Include(h => h.Usuario)
                .Include(h => h.Canteiros)
                .Include(h => h.Ferramentas)
                .OrderBy(h => h.Nome)
                .ToListAsync();
        }

        public async Task<Horta?> GetById(Guid id)
        {
            return await _context.Hortas
                .AsNoTracking()
                .Include(h => h.Endereco)
                .Include(h => h.Usuario)
                .Include(h => h.Canteiros)
                .Include(h => h.Ferramentas)
                .FirstOrDefaultAsync(h => h.Id == id);
        }

        public async Task Salvar(Horta horta)
        {
            var existingHorta = await _context.Hortas.FindAsync(horta.Id);

            if (existingHorta == null)
            {
                await _context.Hortas.AddAsync(horta);
            }
            else
            {
                _context.Entry(existingHorta).CurrentValues.SetValues(horta);
            }
        }
    }
}