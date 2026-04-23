using Colhetiva.Core.Enums;
using System;

namespace Colhetiva.Core.Entities
{
    public class Canteiro
    {
        public Guid Id { get; set; }
        public string Identificacao { get; set; } = string.Empty; // ex: "Lote A1"
        public string Dimensoes { get; set; } = string.Empty; // ex: "2x3m"
        
        // Relacionamento com Horta
        public Guid HortaId { get; set; }
        public Horta Horta { get; set; } = null!;
        
        // Status do canteiro
        public StatusCanteiro Status { get; set; }
    }
}