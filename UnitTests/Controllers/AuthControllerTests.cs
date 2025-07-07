using AutenticacaoAPI.Controllers;
using Core.DTOs;
using Core.Interfaces.Services;
using Core.Requests.Create;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace UnitTests.Controllers
{
    public class AuthControllerTests
    {

        private readonly Mock<IAuthService> _mockAuthService;
        private readonly Mock<IUsuarioService> _mockUsuarioService;
        private readonly AuthController _authController;

        public AuthControllerTests()
        {
            _mockAuthService = new Mock<IAuthService>();
            _mockUsuarioService = new Mock<IUsuarioService>();
            _authController = new AuthController(_mockAuthService.Object, _mockUsuarioService.Object);
        }

        [Fact]
        public void GetToken_ShouldReturnOk_WhenCredentialsAreValid()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Email = "yuri@email.com",
                Senha = "yuri"
            };

            var loginResponse = new LoginDTO
            {
                Email = "yuri@email.com",
                Role = "ADMIN",
                Token = "fake-jwt-token"
            };

            _mockAuthService.Setup(s => s.GetToken(loginRequest)).Returns(loginResponse);

            // Act
            var result = _authController.GetToken(loginRequest);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, ok.StatusCode);
            Assert.Equal(loginResponse, ok.Value);
        }

        [Fact]
        public void GetToken_ShouldReturnUnauthorized_WhenCredentialsInvalid()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Email = "yuri@email.com",
                Senha = "senha-errada"
            };

            _mockAuthService.Setup(s => s.GetToken(loginRequest)).Throws(new UnauthorizedAccessException("Credenciais inválidas"));

            // Act
            var result = _authController.GetToken(loginRequest);

            // Assert
            var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal(401, unauthorized.StatusCode);

            var value = unauthorized.Value?.GetType().GetProperty("mensagem")?.GetValue(unauthorized.Value)?.ToString();
            Assert.Equal("Credenciais inválidas", value);
        }

    }
}
