using Colhetiva.Core.Entities;
using Colhetiva.Core.Interfaces.Repositories;
using Colhetiva.Core.Interfaces.Service;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Colhetiva.Core.Services
{
    public class NotificacaoService : INotificacaoService
    {
        private readonly IUnitOfWork _unitOfWork;

        public NotificacaoService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task<int> GetCountNaoLidasAsync(Guid usuarioId)
            => _unitOfWork.NotificacaoRepository.CountNaoLidasAsync(usuarioId);

        public Task<List<Notificacao>> GetNotificacoesAsync(Guid usuarioId)
            => _unitOfWork.NotificacaoRepository.GetPorUsuarioAsync(usuarioId);

        public async Task<Notificacao?> MarcarComoLidaAsync(Guid notificacaoId, Guid usuarioId)
        {
            var notificacao = await _unitOfWork.NotificacaoRepository.GetByIdAsync(notificacaoId);
            if (notificacao == null || notificacao.UsuarioId != usuarioId)
                return null;

            notificacao.Lida = true;
            notificacao.DataLeitura = DateTime.UtcNow;
            await _unitOfWork.NotificacaoRepository.Salvar(notificacao);
            await _unitOfWork.CompleteAsync();
            return notificacao;
        }
    }
}
