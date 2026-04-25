using Colhetiva.Core.Entities;
using Colhetiva.Core.Interfaces.Repositories;
using Colhetiva.Core.Interfaces.Service;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Colhetiva.Core.Services
{
    public class FerramentaService : IFerramentaService
    {
        private readonly IUnitOfWork _unitOfWork;
        
        public FerramentaService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        
        public async Task<List<Ferramenta>> GetAllAsync()
        {
            return await _unitOfWork.FerramentaRepository.GetFerramentas();
        }
        
        public async Task<Ferramenta?> GetByIdAsync(Guid id)
        {
            return await _unitOfWork.FerramentaRepository.GetById(id);
        }
        
        public async Task<Ferramenta> CriarFerramentaAsync(Ferramenta novaFerramenta)
        {
            novaFerramenta.Id = Guid.NewGuid();
            await _unitOfWork.FerramentaRepository.Salvar(novaFerramenta);
            await _unitOfWork.CompleteAsync();
            return novaFerramenta;
        }
    }
}