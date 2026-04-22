using Colhetiva.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colhetiva.Core.Interfaces.Service
{
    public interface IUsuarioService
    {
        Task<Usuario?> GetById(Guid id);
        Task<Usuario?> GetByEmail(string email);
        Task<Usuario?> Autenticar(string email, string senha);
        Task<Usuario> Salvar(Usuario usuario);
        Task<List<Usuario>> GetAll();
    }
}