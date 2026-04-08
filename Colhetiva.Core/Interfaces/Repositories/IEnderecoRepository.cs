using Colhetiva.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colhetiva.Core.Interfaces.Repositories
{
    public interface IEnderecoRepository
    {
        Task<Endereco?> GetById(Guid id);
        Task Salvar(Endereco endereco);
        Task<List<Endereco>> GetByCep(string cep);
    }
}
