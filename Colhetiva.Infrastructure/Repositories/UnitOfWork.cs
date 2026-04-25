using Colhetiva.Core.Interfaces.Repositories;
using Colhetiva.Infrastructure.Context;
using System;
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
        public IUserContextRepository UserContextRepository { get; }
        public IHortaRepository HortaRepository { get; }
        public ICanteiroRepository CanteiroRepository { get; }
        public ISolicitacaoRepository SolicitacaoRepository { get; }
        public IFerramentaRepository FerramentaRepository { get; }
        public IEmprestimoRepository EmprestimoRepository { get; }

        public UnitOfWork(
            ColhetivaDbContext context,
            IEstadoRepository estadoRepository,
            IUsuarioRepository usuarioRepository,
            ICidadeRepository cidadeRepository,
            IEnderecoRepository enderecoRepository,
            IUserContextRepository userContextRepository,
            IHortaRepository hortaRepository,
            ICanteiroRepository canteiroRepository,
            ISolicitacaoRepository solicitacaoRepository,
            IFerramentaRepository ferramentaRepository,
            IEmprestimoRepository emprestimoRepository)
        {
            _context = context;
            EstadoRepository = estadoRepository;
            UsuarioRepository = usuarioRepository;
            CidadeRepository = cidadeRepository;
            EnderecoRepository = enderecoRepository;
            UserContextRepository = userContextRepository;
            HortaRepository = hortaRepository;
            CanteiroRepository = canteiroRepository;
            SolicitacaoRepository = solicitacaoRepository;
            FerramentaRepository = ferramentaRepository;
            EmprestimoRepository = emprestimoRepository;
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
