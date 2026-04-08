using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colhetiva.Core.Entities
{
    public class Cidade
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public Guid EstadoId { get; set; }
        public Estado? Estado { get; set; }
    }
}
