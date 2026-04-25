using Colhetiva.Core.Entities;
using Colhetiva.Core.Interfaces.Repositories;
using Colhetiva.Core.Interfaces.Service;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Colhetiva.Core.Services
{
    public class EmprestimoService : IEmprestimoService
    {
        private readonly IUnitOfWork _unitOfWork;
        
        public EmprestimoService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        
        public async Task<List<Emprestimo>> GetAllAsync()
        {
            return await _unitOfWork.EmprestimoRepository.GetEmprestimos();
        }
        
        public async Task<Emprestimo?> GetByIdAsync(Guid id)
        {
            return await _unitOfWork.EmprestimoRepository.GetById(id);
        }
        
        public async Task<Emprestimo> CriarEmprestimoAsync(Emprestimo novoEmprestimo)
        {
            novoEmprestimo.Id = Guid.NewGuid();
            novoEmprestimo.DataRetirada = DateTime.UtcNow;
            await _unitOfWork.EmprestimoRepository.Salvar(novoEmprestimo);
            await _unitOfWork.CompleteAsync();
            return novoEmprestimo;
        }
    }
}