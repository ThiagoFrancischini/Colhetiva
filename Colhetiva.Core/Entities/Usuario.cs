using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colhetiva.Core.Entities
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string CPF { get; set; } = string.Empty;
        public string Email { get; set; }
        public string Password { get; set; }
        public Guid EnderecoId { get; set; }
        public Endereco Endereco { get; set; } = new Endereco();
    }
}
