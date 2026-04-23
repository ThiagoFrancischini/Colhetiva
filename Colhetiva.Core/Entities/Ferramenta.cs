using Colhetiva.Core.Enums;
using System;

namespace Colhetiva.Core.Entities
{
    public class Ferramenta
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        
        // Relacionamento com Horta
        public Guid HortaId { get; set; }
        public Horta Horta { get; set; } = null!;
        
        // Status da ferramenta
        public StatusFerramenta Status { get; set; }
    }
}