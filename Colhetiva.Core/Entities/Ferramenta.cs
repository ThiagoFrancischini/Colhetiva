using Colhetiva.Core.Enums;
using System;

namespace Colhetiva.Core.Entities
{
    public class Ferramenta
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        
        public Guid HortaId { get; set; }
        public Horta Horta { get; set; } = null!;
        
        public StatusFerramenta Status { get; set; }
    }
}