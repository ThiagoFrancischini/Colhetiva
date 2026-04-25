using Colhetiva.Core.Entities;
using Colhetiva.Core.Interfaces.Repositories;
using Colhetiva.Core.Interfaces.Service;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Colhetiva.Core.Services
{
    public class CanteiroService : ICanteiroService
    {
        private readonly IUnitOfWork _unitOfWork;
        
        public CanteiroService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        
        public async Task<List<Canteiro>> GetAllAsync()
        {
            return await _unitOfWork.CanteiroRepository.GetCanteiros();
        }
        
        public async Task<Canteiro?> GetByIdAsync(Guid id)
        {
            return await _unitOfWork.CanteiroRepository.GetById(id);
        }
        
        public async Task<Canteiro> CriarCanteiroAsync(Canteiro novoCanteiro)
        {
            novoCanteiro.Id = Guid.NewGuid();
            await _unitOfWork.CanteiroRepository.Salvar(novoCanteiro);
            await _unitOfWork.CompleteAsync();
            return novoCanteiro;
        }
    }
}