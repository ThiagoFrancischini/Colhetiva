using Colhetiva.Core.Entities;
using Colhetiva.Core.Interfaces.Repositories;
using Colhetiva.Core.Interfaces.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colhetiva.Core.Services
{
    public class AvisoService : IAvisoService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AvisoService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Aviso> PublicarAvisoAsync(Aviso novoAviso)
        {
            novoAviso.Id = Guid.NewGuid();
            novoAviso.DataCriacao = DateTime.UtcNow;
            await _unitOfWork.AvisoRepository.Salvar(novoAviso);

            var destinatarios = await _unitOfWork.SolicitacaoRepository
                .GetUsuarioIdsAprovadosPorHortaAsync(novoAviso.HortaId);

            foreach (var usuarioId in destinatarios.Where(id => id != novoAviso.UsuarioId))
            {
                await _unitOfWork.NotificacaoRepository.Salvar(new Notificacao
                {
                    Id = Guid.NewGuid(),
                    UsuarioId = usuarioId,
                    AvisoId = novoAviso.Id,
                    DataCriacao = DateTime.UtcNow,
                    Lida = false
                });
            }

            await _unitOfWork.CompleteAsync();
            return novoAviso;
        }

        public Task<List<Aviso>> GetFeedConsolidadoAsync(Guid usuarioId)
            => _unitOfWork.AvisoRepository.GetFeedPorUsuarioAsync(usuarioId);
    }
}
