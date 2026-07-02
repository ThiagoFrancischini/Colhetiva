using Colhetiva.Core.Entities;
using Colhetiva.Core.Input;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Colhetiva.Core.Interfaces.Service
{
    public interface IHortaService
    {
        Task<List<Horta>> GetAllAsync();
        Task<Horta?> GetByIdAsync(Guid id);
        Task<Horta> CriarHortaAsync(Horta novaHorta);
        Task<Horta> CriarCompletoAsync(HortaCadastroInput input, Guid? organizationId);
        Task AtualizarCompletoAsync(HortaCadastroInput input, Guid? organizationId);
        Task ExcluirAsync(Guid id);
        Task<List<Horta>> FiltrarAsync(string? nome, string? cidade);
    }
}