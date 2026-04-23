using Colhetiva.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Colhetiva.Core.Interfaces.Repositories
{
    public interface ICanteiroRepository
    {
        Task<List<Canteiro>> GetCanteiros();
        Task Salvar(Canteiro canteiro);
        Task<Canteiro?> GetById(Guid id);
    }
}