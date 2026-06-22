using Colhetiva.Core.Entities;
using Colhetiva.Core.Enums;
using Colhetiva.Core.Interfaces.Repositories;
using Colhetiva.Core.Services;
using Moq;
using Xunit;

namespace Colhetiva.Tests
{
    public class UsuarioServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IUsuarioRepository> _mockUsuarioRepository;
        private readonly Mock<IUserContextRepository> _mockUserContextRepository;
        private readonly UsuarioService _service;

        public UsuarioServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockUsuarioRepository = new Mock<IUsuarioRepository>();
            _mockUserContextRepository = new Mock<IUserContextRepository>();

            _mockUnitOfWork.Setup(u => u.UsuarioRepository).Returns(_mockUsuarioRepository.Object);
            _mockUnitOfWork.Setup(u => u.UserContextRepository).Returns(_mockUserContextRepository.Object);
            _mockUnitOfWork.Setup(u => u.CompleteAsync()).ReturnsAsync(1);

            _service = new UsuarioService(_mockUnitOfWork.Object);
        }

        private Usuario CriarUsuarioValido()
        {
            return new Usuario
            {
                Id = Guid.NewGuid(),
                Nome = "João Silva",
                CPF = "12345678901",
                Email = "joao@email.com",
                Password = "senha123",
                EnderecoId = Guid.NewGuid()
            };
        }

        [Fact]
        public async Task Salvar_UsuarioValido_DeveCriarComSucesso()
        {
            // Arrange
            var usuario = CriarUsuarioValido();
            _mockUsuarioRepository.Setup(r => r.GetByEmail(usuario.Email)).ReturnsAsync((Usuario?)null);
            _mockUsuarioRepository.Setup(r => r.Salvar(It.IsAny<Usuario>())).Returns(Task.CompletedTask);
            _mockUserContextRepository.Setup(r => r.AddAsync(It.IsAny<UserContext>())).Returns(Task.CompletedTask);

            // Act
            var resultado = await _service.Salvar(usuario);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(usuario.Email, resultado.Email);
            _mockUsuarioRepository.Verify(r => r.Salvar(It.IsAny<Usuario>()), Times.Once);
            _mockUserContextRepository.Verify(r => r.AddAsync(It.IsAny<UserContext>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Once);
        }

        [Fact]
        public async Task Salvar_UsuarioSemNome_DeveLancarExcecao()
        {
            // Arrange
            var usuario = new Usuario
            {
                Nome = "",
                CPF = "12345678901",
                Email = "joao@email.com",
                Password = "senha123",
                EnderecoId = Guid.NewGuid()
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.Salvar(usuario));
        }

        [Fact]
        public async Task Salvar_CPFInvalido_DeveLancarExcecao()
        {
            // Arrange
            var usuario = new Usuario
            {
                Nome = "João Silva",
                CPF = "123", // CPF inválido (menos de 11 dígitos)
                Email = "joao@email.com",
                Password = "senha123",
                EnderecoId = Guid.NewGuid()
            };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _service.Salvar(usuario));
            Assert.Contains("CPF deve conter exatamente 11 dígitos", ex.Message);
        }

        [Fact]
        public async Task Salvar_CPFTodosDigitosIguais_DeveLancarExcecao()
        {
            // Arrange
            var usuario = new Usuario
            {
                Nome = "João Silva",
                CPF = "11111111111", // CPF inválido (todos dígitos iguais)
                Email = "joao@email.com",
                Password = "senha123",
                EnderecoId = Guid.NewGuid()
            };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _service.Salvar(usuario));
            Assert.Contains("CPF inválido", ex.Message);
        }

        [Fact]
        public async Task Salvar_EmailInvalido_DeveLancarExcecao()
        {
            // Arrange
            var usuario = new Usuario
            {
                Nome = "João Silva",
                CPF = "12345678901",
                Email = "email-invalido", // Email sem @
                Password = "senha123",
                EnderecoId = Guid.NewGuid()
            };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _service.Salvar(usuario));
            Assert.Contains("Email inválido", ex.Message);
        }

        [Fact]
        public async Task Salvar_SenhaCurta_DeveLancarExcecao()
        {
            // Arrange
            var usuario = new Usuario
            {
                Nome = "João Silva",
                CPF = "12345678901",
                Email = "joao@email.com",
                Password = "123", // Senha muito curta
                EnderecoId = Guid.NewGuid()
            };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _service.Salvar(usuario));
            Assert.Contains("Senha deve ter pelo menos 6 caracteres", ex.Message);
        }

        [Fact]
        public async Task Salvar_EmailDuplicado_DeveLancarExcecao()
        {
            // Arrange
            var usuarioExistente = new Usuario
            {
                Id = Guid.NewGuid(),
                Nome = "Maria Silva",
                CPF = "98765432109",
                Email = "joao@email.com",
                Password = "outrasenha",
                EnderecoId = Guid.NewGuid()
            };

            var novoUsuario = new Usuario
            {
                Id = Guid.NewGuid(),
                Nome = "João Silva",
                CPF = "12345678901",
                Email = "joao@email.com",
                Password = "senha123",
                EnderecoId = Guid.NewGuid()
            };

            _mockUsuarioRepository.Setup(r => r.GetByEmail(novoUsuario.Email)).ReturnsAsync(usuarioExistente);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.Salvar(novoUsuario));
            Assert.Contains("Email já está em uso", ex.Message);
        }

        [Fact]
        public async Task Salvar_OrganizacaoSemCPF_DeveCriarComSucesso()
        {
            // Arrange
            var organizacao = new Usuario
            {
                Id = Guid.NewGuid(),
                Nome = "Prefeitura Municipal",
                CPF = "",
                Email = "prefeitura@prefeitura.com",
                Password = "senha123",
                EnderecoId = Guid.NewGuid()
            };

            _mockUsuarioRepository.Setup(r => r.GetByEmail(organizacao.Email)).ReturnsAsync((Usuario?)null);
            _mockUsuarioRepository.Setup(r => r.Salvar(It.IsAny<Usuario>())).Returns(Task.CompletedTask);
            _mockUserContextRepository.Setup(r => r.AddAsync(It.IsAny<UserContext>())).Returns(Task.CompletedTask);

            // Act
            var resultado = await _service.Salvar(organizacao, Role.ADMIN);

            // Assert
            Assert.NotNull(resultado);
            _mockUserContextRepository.Verify(r => r.AddAsync(It.Is<UserContext>(c => c.Role == Role.ADMIN)), Times.Once);
        }

        [Fact]
        public async Task Salvar_EnderecoNaoInformado_DeveLancarExcecao()
        {
            // Arrange
            var usuario = new Usuario
            {
                Nome = "João Silva",
                CPF = "12345678901",
                Email = "joao@email.com",
                Password = "senha123",
                EnderecoId = Guid.Empty // Endereço não informado
            };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _service.Salvar(usuario));
            Assert.Contains("Endereço deve ser informado", ex.Message);
        }

        [Fact]
        public async Task Salvar_UsuarioSemCPFNaoAdmin_DeveLancarExcecao()
        {
            // Arrange - Usuário comum sem CPF
            var usuario = new Usuario
            {
                Nome = "João Silva",
                CPF = "",
                Email = "joao@email.com",
                Password = "senha123",
                EnderecoId = Guid.NewGuid()
            };

            // Act & Assert - Deve falhar pois não é ADMIN e não tem CPF
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _service.Salvar(usuario, Role.PARTICIPANT));
            Assert.Contains("CPF é obrigatório", ex.Message);
        }
    }
}