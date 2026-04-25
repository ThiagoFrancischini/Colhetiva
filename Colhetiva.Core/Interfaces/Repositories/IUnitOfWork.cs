using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colhetiva.Core.Interfaces.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IEstadoRepository EstadoRepository { get; }
        IUsuarioRepository UsuarioRepository { get; }
        ICidadeRepository CidadeRepository { get; }
        IEnderecoRepository EnderecoRepository { get; }
        IUserContextRepository UserContextRepository { get; }
        IHortaRepository HortaRepository { get; }
        ICanteiroRepository CanteiroRepository { get; }
        ISolicitacaoRepository SolicitacaoRepository { get; }
        IFerramentaRepository FerramentaRepository { get; }
        IEmprestimoRepository EmprestimoRepository { get; }
        Task<int> CompleteAsync();
    }
}
