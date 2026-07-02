using System;

namespace Colhetiva.Core.Entities
{
    public class Notificacao
    {
        public Guid Id { get; set; }

        public Guid UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;

        public Guid AvisoId { get; set; }
        public Aviso Aviso { get; set; } = null!;

        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
        public bool Lida { get; set; }
        public DateTime? DataLeitura { get; set; }
    }
}
