using Colhetiva.Core.Entities;
using Colhetiva.Core.Interfaces.Repositories;
using Colhetiva.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Colhetiva.Infrastructure.Repositories
{
    public class NotificacaoRepository : INotificacaoRepository
    {
        private readonly ColhetivaDbContext _context;
        public NotificacaoRepository(ColhetivaDbContext context)
        {
            _context = context;
        }

        public async Task Salvar(Notificacao notificacao)
        {
            var existingNotificacao = await _context.Notificacoes.FindAsync(notificacao.Id);

            if (existingNotificacao == null)
            {
                await _context.Notificacoes.AddAsync(notificacao);
            }
            else
            {
                _context.Entry(existingNotificacao).CurrentValues.SetValues(notificacao);
            }
        }

        public async Task<Notificacao?> GetByIdAsync(Guid id)
        {
            return await _context.Notificacoes
                .Include(n => n.Aviso)
                    .ThenInclude(a => a.Horta)
                .FirstOrDefaultAsync(n => n.Id == id);
        }

        public async Task<List<Notificacao>> GetPorUsuarioAsync(Guid usuarioId)
        {
            return await _context.Notificacoes
                .AsNoTracking()
                .Include(n => n.Aviso)
                    .ThenInclude(a => a.Horta)
                .Where(n => n.UsuarioId == usuarioId)
                .OrderByDescending(n => n.DataCriacao)
                .ToListAsync();
        }

        public async Task<int> CountNaoLidasAsync(Guid usuarioId)
        {
            return await _context.Notificacoes
                .CountAsync(n => n.UsuarioId == usuarioId && !n.Lida);
        }
    }
}
