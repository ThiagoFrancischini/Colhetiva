using Colhetiva.Core.Entities;
using Colhetiva.Core.Interfaces.Repositories;
using Colhetiva.Core.Interfaces.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colhetiva.Core.Services
{
    public class EstadoService : IEstadoService
    {
        private readonly IUnitOfWork _unitOfWork;

        public EstadoService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<Estado>> GetAllAsync()
        {
            return await _unitOfWork.EstadoRepository.GetEstados();
        }

        public async Task<Estado?> GetByIdAsync(Guid id)
        {
            return await _unitOfWork.EstadoRepository.GetById(id);
        }

        public async Task<Estado> CriarEstadoAsync(Estado novoEstado)
        {
            novoEstado.Id = Guid.NewGuid();

            await _unitOfWork.EstadoRepository.Salvar(novoEstado);
            await _unitOfWork.CompleteAsync();

            return novoEstado;
        }
    }
}
