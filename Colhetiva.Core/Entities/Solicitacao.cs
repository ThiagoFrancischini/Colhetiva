using Colhetiva.Core.Enums;
using System;

namespace Colhetiva.Core.Entities
{
    public class Solicitacao
    {
        public Guid Id { get; set; }
        public DateTime DataPedido { get; set; } = DateTime.UtcNow;
        
        public StatusSolicitacao Status { get; set; }
        
        public Guid UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;
        
        public Guid CanteiroId { get; set; }
        public Canteiro Canteiro { get; set; } = null!;
    }
}