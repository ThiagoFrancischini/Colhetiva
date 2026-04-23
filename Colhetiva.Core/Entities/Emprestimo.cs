using System;

namespace Colhetiva.Core.Entities
{
    public class Emprestimo
    {
        public Guid Id { get; set; }
        public DateTime DataRetirada { get; set; } = DateTime.UtcNow;
        public DateTime? DataDevolucao { get; set; } // anulável
        
        // Relacionamento com Usuario (voluntário)
        public Guid UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;
        
        // Relacionamento com Ferramenta
        public Guid FerramentaId { get; set; }
        public Ferramenta Ferramenta { get; set; } = null!;
    }
}