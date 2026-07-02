using System;

namespace Colhetiva.Core.Entities
{
    public class Aviso
    {
        public Guid Id { get; set; }

        public Guid HortaId { get; set; }
        public Horta Horta { get; set; } = null!;

        public Guid UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;

        public string Titulo { get; set; } = string.Empty;
        public string Conteudo { get; set; } = string.Empty;
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    }
}
