using Colhetiva.Core.Entities;
using Colhetiva.Core.Enums;
using Colhetiva.Core.Interfaces.Repositories;
using Colhetiva.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Colhetiva.Infrastructure.Repositories
{
    public class AvisoRepository : IAvisoRepository
    {
        private readonly ColhetivaDbContext _context;
        public AvisoRepository(ColhetivaDbContext context)
        {
            _context = context;
        }

        public async Task Salvar(Aviso aviso)
        {
            var existingAviso = await _context.Avisos.FindAsync(aviso.Id);

            if (existingAviso == null)
            {
                await _context.Avisos.AddAsync(aviso);
            }
            else
            {
                _context.Entry(existingAviso).CurrentValues.SetValues(aviso);
            }
        }

        public async Task<Aviso?> GetById(Guid id)
        {
            return await _context.Avisos
                .AsNoTracking()
                .Include(a => a.Horta)
                .Include(a => a.Usuario)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<List<Aviso>> GetFeedPorUsuarioAsync(Guid usuarioId)
        {
            return await _context.Avisos
                .AsNoTracking()
                .Include(a => a.Horta)
                .Include(a => a.Usuario)
                .Where(a =>
                    a.Horta.UsuarioId == usuarioId ||
                    _context.Solicitacoes.Any(s =>
                        s.UsuarioId == usuarioId &&
                        s.Status == StatusSolicitacao.Aprovado &&
                        s.Canteiro.HortaId == a.HortaId))
                .OrderByDescending(a => a.DataCriacao)
                .ToListAsync();
        }
    }
}
