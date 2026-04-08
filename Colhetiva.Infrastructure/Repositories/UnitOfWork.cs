using Colhetiva.Core.Interfaces.Repositories;
using Colhetiva.Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colhetiva.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ColhetivaDbContext _context;
        public IEstadoRepository EstadoRepository { get; }
        public IUsuarioRepository UsuarioRepository { get; }
        public ICidadeRepository CidadeRepository { get; }
        public IEnderecoRepository EnderecoRepository { get; }
        public UnitOfWork(
            ColhetivaDbContext context)
        {
            _context = context;
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
