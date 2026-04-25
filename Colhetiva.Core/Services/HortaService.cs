using Colhetiva.Core.Entities;
using Colhetiva.Core.Interfaces.Repositories;
using Colhetiva.Core.Interfaces.Service;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Colhetiva.Core.Services
{
    public class HortaService : IHortaService
    {
        private readonly IUnitOfWork _unitOfWork;
        
        public HortaService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        
        public async Task<List<Horta>> GetAllAsync()
        {
            return await _unitOfWork.HortaRepository.GetHortas();
        }
        
        public async Task<Horta?> GetByIdAsync(Guid id)
        {
            return await _unitOfWork.HortaRepository.GetById(id);
        }
        
        public async Task<Horta> CriarHortaAsync(Horta novaHorta)
        {
            novaHorta.Id = Guid.NewGuid();
            await _unitOfWork.HortaRepository.Salvar(novaHorta);
            await _unitOfWork.CompleteAsync();
            return novaHorta;
        }
    }
}