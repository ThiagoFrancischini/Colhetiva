using Colhetiva.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Colhetiva.Core.Interfaces.Repositories
{
    public interface IEmprestimoRepository
    {
        Task<List<Emprestimo>> GetEmprestimos();
        Task Salvar(Emprestimo emprestimo);
        Task<Emprestimo?> GetById(Guid id);
    }
}