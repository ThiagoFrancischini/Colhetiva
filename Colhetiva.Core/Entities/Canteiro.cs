using Colhetiva.Core.Enums;
using System;

namespace Colhetiva.Core.Entities
{
    public class Canteiro
    {
        public Guid Id { get; set; }
        public string Identificacao { get; set; } = string.Empty;
        public string Dimensoes { get; set; } = string.Empty;
        
        public Guid HortaId { get; set; }
        public Horta Horta { get; set; } = null!;
        
        public StatusCanteiro Status { get; set; }
    }
}