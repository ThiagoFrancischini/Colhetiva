using Colhetiva.Core.Enums;
using System;

namespace Colhetiva.Core.Entities
{
    public class Solicitacao
    {
        public Guid Id { get; set; }
        public DateTime DataPedido { get; set; } = DateTime.UtcNow;
        
        // Status da solicitação
        public StatusSolicitacao Status { get; set; }
        
        // Relacionamento com Usuario (solicitante)
        public Guid UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;
        
        // Relacionamento com Canteiro
        public Guid CanteiroId { get; set; }
        public Canteiro Canteiro { get; set; } = null!;
    }
}