using Colhetiva.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colhetiva.Core.Interfaces.Service
{
    public interface IEnderecoService
    {
        Task<Endereco?> GetById(Guid id);
        Task<Endereco> Salvar(Endereco endereco);
        Task<List<Endereco>> GetByCep(string cep);
    }
}