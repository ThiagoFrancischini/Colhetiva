using System;

namespace Colhetiva.Core.Entities
{
    public class RegistroAtividade
    {
        public Guid Id { get; set; }

        public Guid UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;

        public Guid HortaId { get; set; }
        public Horta Horta { get; set; } = null!;

        public Guid? CanteiroId { get; set; }
        public Canteiro? Canteiro { get; set; }

        public DateTime DataHora { get; set; }
        public string Atividade { get; set; } = string.Empty;
        public string? Observacoes { get; set; }
        public string? FotoUrl { get; set; }
    }
}
