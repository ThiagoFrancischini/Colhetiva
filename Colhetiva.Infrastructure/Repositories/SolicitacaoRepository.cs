using Colhetiva.Core.Entities;
using Colhetiva.Core.Interfaces.Repositories;
using Colhetiva.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Colhetiva.Infrastructure.Repositories
{
    public class SolicitacaoRepository : ISolicitacaoRepository
    {
        private readonly ColhetivaDbContext _context;
        public SolicitacaoRepository(ColhetivaDbContext context)
        {
            _context = context;
        }

        public async Task<List<Solicitacao>> GetSolicitacoes()
        {
            return await _context
                .Solicitacoes
                .Include(s => s.Usuario)
                .Include(s => s.Canteiro)
                .ThenInclude(c => c.Horta)
                .OrderByDescending(s => s.DataPedido)
                .ToListAsync();
        }

        public async Task<Solicitacao?> GetById(Guid id)
        {
            return await _context.Solicitacoes
                .AsNoTracking()
                .Include(s => s.Usuario)
                .Include(s => s.Canteiro)
                .ThenInclude(c => c.Horta)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task Salvar(Solicitacao solicitacao)
        {
            var existingSolicitacao = await _context.Solicitacoes.FindAsync(solicitacao.Id);

            if (existingSolicitacao == null)
            {
                await _context.Solicitacoes.AddAsync(solicitacao);
            }
            else
            {
                _context.Entry(existingSolicitacao).CurrentValues.SetValues(solicitacao);
            }
        }
    }
}