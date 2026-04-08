using Colhetiva.Core.Entities;
using Colhetiva.Core.Interfaces.Repositories;
using Colhetiva.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;


namespace Colhetiva.Infrastructure.Repositories
{
    public class EnderecoRepository : IEnderecoRepository
    {
        private readonly ColhetivaDbContext _context;

        public EnderecoRepository(ColhetivaDbContext context)
        {
            _context = context;
        }

        public async Task<Endereco?> GetById(Guid id)
        {
            return await _context.Enderecos
                .Include(e => e.Cidade)
                .ThenInclude(c => c.Estado)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<List<Endereco>> GetByCep(string cep)
        {
            return await _context.Enderecos
                .Where(e => e.Cep == cep)
                .Include(e => e.Cidade)
                .ThenInclude(c => c.Estado)
                .ToListAsync();
        }

        public async Task Salvar(Endereco endereco)
        {
            var existingEndereco = await _context.Enderecos.FindAsync(endereco.Id);

            if (existingEndereco == null)
            {
                await _context.Enderecos.AddAsync(endereco);
            }
            else
            {
                _context.Entry(existingEndereco).CurrentValues.SetValues(endereco);
            }
        }
    }
}
