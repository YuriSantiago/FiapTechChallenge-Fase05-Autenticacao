using Core.Entities;
using Core.Helper;
using Core.Interfaces.Repositories;
using Core.Requests.Create;
using Core.Services;
using Microsoft.Extensions.Configuration;
using Moq;

namespace UnitTests.Services
{
    public class AuthServiceTests
    {

        private readonly Mock<IUsuarioRepository> _usuarioRepositoryMock;
        private readonly IConfiguration _configuration;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _usuarioRepositoryMock = new Mock<IUsuarioRepository>();

            var inMemorySettings = new Dictionary<string, string?>
            {
                { "Jwt:Secret", "SuperChaveUltraSeguraComMaisDe32Caracteres" }
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _authService = new AuthService(_configuration, _usuarioRepositoryMock.Object);
        }

        [Fact]
        public void GetToken_ShouldReturnToken_WhenCredentialsAreValid()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Email = "yuri@email.com",
                Senha = "senha123"
            };

            var senha = Base64Helper.Encode(loginRequest.Senha);

            var usuario = new Usuario
            {
                Nome = "Yuri Santiago",
                Email = loginRequest.Email,
                Senha = senha,
                Role = "ADMIN"
            };

            _usuarioRepositoryMock.Setup(repo => repo.GetAll()).Returns([usuario]);

            // Act
            var result = _authService.GetToken(loginRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(usuario.Email, result.Email);
            Assert.Equal(usuario.Role, result.Role);
            Assert.False(string.IsNullOrWhiteSpace(result.Token));
        }

        [Fact]
        public void GetToken_ShouldThrowUnauthorized_WhenUserNotFound()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Email = "naoexiste@email.com",
                Senha = "qualquer"
            };

            _usuarioRepositoryMock.Setup(repo => repo.GetAll()).Returns([]);

            // Act & Assert
            var ex = Assert.Throws<UnauthorizedAccessException>(() => _authService.GetToken(loginRequest));
            Assert.Equal("Usuário não encontrado", ex.Message);
        }

        [Fact]
        public void GetToken_ShouldThrowUnauthorized_WhenPasswordIsInvalid()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Email = "yuri@email.com",
                Senha = "senhaErrada"
            };

            var usuario = new Usuario
            {
                Nome = "Yuri Santiago",
                Email = loginRequest.Email,
                Senha = Base64Helper.Encode("senhaCorreta"),
                Role = "ADMIN"
            };

            _usuarioRepositoryMock.Setup(repo => repo.GetAll()).Returns([usuario]);

            // Act & Assert
            var ex = Assert.Throws<UnauthorizedAccessException>(() => _authService.GetToken(loginRequest));
            Assert.Equal("Senha Inválida", ex.Message);
        }


    }
}
