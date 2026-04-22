using Colhetiva.Core.Entities;
using Colhetiva.Core.Enums;
using Colhetiva.Core.Interfaces.Repositories;
using Colhetiva.Core.Interfaces.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Colhetiva.Core.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UsuarioService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Usuario?> GetById(Guid id)
        {
            return await _unitOfWork.UsuarioRepository.GetById(id);
        }

        public async Task<Usuario?> GetByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be null or empty", nameof(email));

            return await _unitOfWork.UsuarioRepository.GetByEmail(email);
        }

        public async Task<Usuario?> Autenticar(string email, string senha)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be null or empty", nameof(email));

            if (string.IsNullOrWhiteSpace(senha))
                throw new ArgumentException("Password cannot be null or empty", nameof(senha));

            return await _unitOfWork.UsuarioRepository.Autenticar(email, senha);
        }

        public async Task<Usuario> Salvar(Usuario usuario)
        {
            ValidarUsuarioParaInsercao(usuario);

            var usuarioExistente = await _unitOfWork.UsuarioRepository.GetByEmail(usuario.Email);
            if (usuarioExistente != null && usuarioExistente.Id != usuario.Id)
            {
                throw new InvalidOperationException("Email já está em uso por outro usuário");
            }

            if (usuario.Id == Guid.Empty)
            {
                usuario.Id = Guid.NewGuid();
            }

            await _unitOfWork.UsuarioRepository.Salvar(usuario);
            
            await _unitOfWork.UserContextRepository.AddAsync(new UserContext
            {
                Id = Guid.NewGuid(),
                UsuarioId = usuario.Id,
                Role = Role.PARTICIPANT
            });

            await _unitOfWork.CompleteAsync();
            
            return usuario;
        }

        public async Task<List<Usuario>> GetAll()
        {
            return await _unitOfWork.UsuarioRepository.GetAll();
        }

        private void ValidarUsuarioParaInsercao(Usuario usuario)
        {
            if (usuario == null)
                throw new ArgumentNullException(nameof(usuario));

            if (string.IsNullOrWhiteSpace(usuario.Nome))
                throw new ArgumentException("Nome é obrigatório", nameof(usuario.Nome));

            if (usuario.Nome.Length > 150)
                throw new ArgumentException("Nome deve ter no máximo 150 caracteres", nameof(usuario.Nome));

            if (string.IsNullOrWhiteSpace(usuario.CPF))
                throw new ArgumentException("CPF é obrigatório", nameof(usuario.CPF));

            var cpfNumerico = new string(usuario.CPF.Where(char.IsDigit).ToArray());
            if (cpfNumerico.Length != 11)
                throw new ArgumentException("CPF deve conter exatamente 11 dígitos", nameof(usuario.CPF));

            if (cpfNumerico.All(c => c == cpfNumerico[0]))
                throw new ArgumentException("CPF inválido", nameof(usuario.CPF));

            if (string.IsNullOrWhiteSpace(usuario.Email))
                throw new ArgumentException("Email é obrigatório", nameof(usuario.Email));

            if (!IsValidEmail(usuario.Email))
                throw new ArgumentException("Email inválido", nameof(usuario.Email));

            if (usuario.Email.Length > 100)
                throw new ArgumentException("Email deve ter no máximo 100 caracteres", nameof(usuario.Email));

            if (string.IsNullOrWhiteSpace(usuario.Password))
                throw new ArgumentException("Senha é obrigatória", nameof(usuario.Password));

            if (usuario.Password.Length < 6)
                throw new ArgumentException("Senha deve ter pelo menos 6 caracteres", nameof(usuario.Password));

            if (usuario.EnderecoId == Guid.Empty)
                throw new ArgumentException("Endereço deve ser informado", nameof(usuario.EnderecoId));
        }

        private bool IsValidEmail(string email)
        {
            // Expressão regular básica para validação de email
            var pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
        }
    }
}