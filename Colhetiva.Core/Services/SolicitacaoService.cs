using Colhetiva.Core.Entities;
using Colhetiva.Core.Interfaces.Repositories;
using Colhetiva.Core.Interfaces.Service;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Colhetiva.Core.Services
{
    public class SolicitacaoService : ISolicitacaoService
    {
        private readonly IUnitOfWork _unitOfWork;
        
        public SolicitacaoService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        
        public async Task<List<Solicitacao>> GetAllAsync()
        {
            return await _unitOfWork.SolicitacaoRepository.GetSolicitacoes();
        }
        
        public async Task<Solicitacao?> GetByIdAsync(Guid id)
        {
            return await _unitOfWork.SolicitacaoRepository.GetById(id);
        }
        
        public async Task<Solicitacao> CriarSolicitacaoAsync(Solicitacao novaSolicitacao)
        {
            novaSolicitacao.Id = Guid.NewGuid();
            novaSolicitacao.DataPedido = DateTime.UtcNow;
            await _unitOfWork.SolicitacaoRepository.Salvar(novaSolicitacao);
            await _unitOfWork.CompleteAsync();
            return novaSolicitacao;
        }
    }
}