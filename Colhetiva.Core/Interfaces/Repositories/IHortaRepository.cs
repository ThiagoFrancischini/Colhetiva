using Colhetiva.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Colhetiva.Core.Interfaces.Repositories
{
    public interface IHortaRepository
    {
        Task<List<Horta>> GetHortas();
        Task Salvar(Horta horta);
        Task<Horta?> GetById(Guid id);
    }
}