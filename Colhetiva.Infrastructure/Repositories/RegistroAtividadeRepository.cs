using Colhetiva.Core.Entities;
using Colhetiva.Core.Interfaces.Repositories;
using Colhetiva.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Colhetiva.Infrastructure.Repositories
{
    public class RegistroAtividadeRepository : IRegistroAtividadeRepository
    {
        private readonly ColhetivaDbContext _context;
        public RegistroAtividadeRepository(ColhetivaDbContext context)
        {
            _context = context;
        }

        public async Task<List<RegistroAtividade>> GetDiarioPessoal(Guid usuarioId, DateTime inicioUtc, DateTime fimUtc)
        {
            return await _context.RegistrosAtividades
                .Include(r => r.Horta)
                .Include(r => r.Canteiro)
                .Where(r => r.UsuarioId == usuarioId && r.DataHora >= inicioUtc && r.DataHora < fimUtc)
                .OrderByDescending(r => r.DataHora)
                .ToListAsync();
        }

        public async Task<List<RegistroAtividade>> GetTimelineHorta(Guid hortaId, int take = 200)
        {
            return await _context.RegistrosAtividades
                .Include(r => r.Usuario)
                .Include(r => r.Canteiro)
                .Where(r => r.HortaId == hortaId)
                .OrderByDescending(r => r.DataHora)
                .Take(take)
                .ToListAsync();
        }

        public async Task<RegistroAtividade?> GetById(Guid id)
        {
            return await _context.RegistrosAtividades
                .AsNoTracking()
                .Include(r => r.Usuario)
                .Include(r => r.Horta)
                .Include(r => r.Canteiro)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task Salvar(RegistroAtividade registro)
        {
            var existingRegistro = await _context.RegistrosAtividades.FindAsync(registro.Id);

            if (existingRegistro == null)
            {
                await _context.RegistrosAtividades.AddAsync(registro);
            }
            else
            {
                _context.Entry(existingRegistro).CurrentValues.SetValues(registro);
            }
        }
    }
}
