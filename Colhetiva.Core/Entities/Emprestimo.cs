using System;
using Colhetiva.Core.Enums;

namespace Colhetiva.Core.Entities
{
    public class Emprestimo
    {
        public Guid Id { get; set; }
        public DateTime DataRetirada { get; set; } = DateTime.UtcNow;
        public DateTime? DataDevolucao { get; set; }

        public StatusEmprestimo Status { get; set; } = StatusEmprestimo.Pendente;

        public Guid UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;

        public Guid FerramentaId { get; set; }
        public Ferramenta Ferramenta { get; set; } = null!;
    }
}