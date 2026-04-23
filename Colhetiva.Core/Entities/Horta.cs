using Colhetiva.Core.Enums;
using System;
using System.Collections.Generic;

namespace Colhetiva.Core.Entities
{
    public class Horta
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Regras { get; set; } = string.Empty;
        
        public Guid EnderecoId { get; set; }
        public Endereco Endereco { get; set; } = null!;
        
        public Guid UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;
        
        public ICollection<Canteiro> Canteiros { get; set; } = new List<Canteiro>();
        public ICollection<Ferramenta> Ferramentas { get; set; } = new List<Ferramenta>();
    }
}