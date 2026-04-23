using Colhetiva.Core.Entities;
using Colhetiva.Core.Interfaces.Repositories;
using Colhetiva.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Colhetiva.Infrastructure.Repositories
{
    public class EmprestimoRepository : IEmprestimoRepository
    {
        private readonly ColhetivaDbContext _context;
        public EmprestimoRepository(ColhetivaDbContext context)
        {
            _context = context;
        }

        public async Task<List<Emprestimo>> GetEmprestimos()
        {
            return await _context
                .Emprestimos
                .Include(e => e.Usuario)
                .Include(e => e.Ferramenta)
                .OrderByDescending(e => e.DataRetirada)
                .ToListAsync();
        }

        public async Task<Emprestimo?> GetById(Guid id)
        {
            return await _context.Emprestimos
                .AsNoTracking()
                .Include(e => e.Usuario)
                .Include(e => e.Ferramenta)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task Salvar(Emprestimo emprestimo)
        {
            var existingEmprestimo = await _context.Emprestimos.FindAsync(emprestimo.Id);

            if (existingEmprestimo == null)
            {
                await _context.Emprestimos.AddAsync(emprestimo);
            }
            else
            {
                _context.Entry(existingEmprestimo).CurrentValues.SetValues(emprestimo);
            }
        }
    }
}