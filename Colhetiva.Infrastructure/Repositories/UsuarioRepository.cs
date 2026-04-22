using Colhetiva.Core.Entities;
using Colhetiva.Core.Interfaces.Repositories;
using Colhetiva.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colhetiva.Infrastructure.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly ColhetivaDbContext _context;

        public UsuarioRepository(ColhetivaDbContext context)
        {
            _context = context;
        }

        public async Task<Usuario?> GetById(Guid id)
        {
            return await _context.Usuarios
                .Include(u => u.Endereco)
                .ThenInclude(e => e.Cidade)
                .ThenInclude(c => c.Estado)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<Usuario?> GetByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            return await _context.Usuarios
                .Include(u => u.Endereco)
                .ThenInclude(e => e.Cidade)
                .ThenInclude(c => c.Estado)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<Usuario?> Autenticar(string email, string senha)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
                return null;

            return await _context.Usuarios
                .Include(u => u.Endereco)
                .ThenInclude(e => e.Cidade)
                .ThenInclude(c => c.Estado)
                .FirstOrDefaultAsync(u => u.Email == email && u.Password == senha);
        }

        public async Task Salvar(Usuario usuario)
        {
            var existingUsuario = await _context.Usuarios.FindAsync(usuario.Id);

            if (existingUsuario == null)
            {
                await _context.Usuarios.AddAsync(usuario);
            }
            else
            {
                _context.Entry(existingUsuario).CurrentValues.SetValues(usuario);
                
                if (existingUsuario.EnderecoId != usuario.EnderecoId)
                {
                    existingUsuario.EnderecoId = usuario.EnderecoId;
                }
            }
        }

        public async Task<List<Usuario>> GetAll()
        {
            return await _context.Usuarios
                .Include(u => u.Endereco)
                .ThenInclude(e => e.Cidade)
                .ThenInclude(c => c.Estado)
                .ToListAsync();
        }
    }
}