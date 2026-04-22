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
    public class EnderecoService : IEnderecoService
    {
        private readonly IUnitOfWork _unitOfWork;

        public EnderecoService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Endereco?> GetById(Guid id)
        {
            return await _unitOfWork.EnderecoRepository.GetById(id);
        }

        public async Task<Endereco> Salvar(Endereco endereco)
        {
            await _unitOfWork.EnderecoRepository.Salvar(endereco);
            return endereco;
        }

        public async Task<List<Endereco>> GetByCep(string cep)
        {
            return await _unitOfWork.EnderecoRepository.GetByCep(cep);
        }
    }
}