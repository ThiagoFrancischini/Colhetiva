using System;
using System.Collections.Generic;

namespace Colhetiva.Core.Entities
{
    public class Organization
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Cnpj { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;

        public Guid? EnderecoId { get; set; }
        public Endereco? Endereco { get; set; }

        public ICollection<Horta> Hortas { get; set; } = new List<Horta>();
        public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    }
}