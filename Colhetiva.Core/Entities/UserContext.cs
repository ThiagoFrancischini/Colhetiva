using Colhetiva.Core.Enums;
using System;

namespace Colhetiva.Core.Entities
{
    public class UserContext
    {
        public Guid Id { get; set; }
        public Guid UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;
        public Role Role { get; set; }
        public Guid? HortaId { get; set; }

        public Horta? Horta { get; set; }
    }
}
