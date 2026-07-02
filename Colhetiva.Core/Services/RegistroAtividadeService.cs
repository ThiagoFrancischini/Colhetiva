using Colhetiva.Core.Entities;
using Colhetiva.Core.Interfaces.Repositories;
using Colhetiva.Core.Interfaces.Service;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Colhetiva.Core.Services
{
    public class RegistroAtividadeService : IRegistroAtividadeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RegistroAtividadeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<RegistroAtividade> CriarAtividadeAsync(RegistroAtividade novoRegistro)
        {
            novoRegistro.Id = Guid.NewGuid();
            await _unitOfWork.RegistroAtividadeRepository.Salvar(novoRegistro);
            await _unitOfWork.CompleteAsync();
            return novoRegistro;
        }

        public Task<List<RegistroAtividade>> GetDiarioPessoalAsync(Guid usuarioId, int ano, int mes)
        {
            var inicioUtc = new DateTime(ano, mes, 1, 0, 0, 0, DateTimeKind.Utc);
            var fimUtc = inicioUtc.AddMonths(1);
            return _unitOfWork.RegistroAtividadeRepository.GetDiarioPessoal(usuarioId, inicioUtc, fimUtc);
        }

        public Task<List<RegistroAtividade>> GetTimelineHortaAsync(Guid hortaId)
            => _unitOfWork.RegistroAtividadeRepository.GetTimelineHorta(hortaId);
    }
}
