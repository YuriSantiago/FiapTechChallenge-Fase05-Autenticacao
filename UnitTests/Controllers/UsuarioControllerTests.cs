using Core.DTOs;
using Core.Helpers;
using Core.Interfaces.Services;
using Core.Requests.Create;
using Core.Requests.Delete;
using Core.Requests.Update;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using UsuarioProdutor.Controllers;

namespace UnitTests.Controllers
{
    public class UsuarioControllerTests
    {
        private readonly Mock<IBus> _mockBus;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<IUsuarioService> _mockUsuarioService;
        private readonly UsuarioController _usuarioController;

        public UsuarioControllerTests()
        {
            _mockBus = new Mock<IBus>();
            _mockConfiguration = new Mock<IConfiguration>();
            _mockUsuarioService = new Mock<IUsuarioService>();
            _usuarioController = new UsuarioController(_mockBus.Object, _mockConfiguration.Object, _mockUsuarioService.Object);
        }

        [Fact]
        public void GetAll_ShouldReturnOkWithUsuarios()
        {
            // Arrange
            var usuarios = new List<UsuarioDTO>
            {
               new() {
                Id = 1,
                Nome = "Yuri Santiago",
                Email = "yuri@email.com",
                Senha = Base64Helper.Encode("yuri"),
                Role = "ADMIN"
               }
             };

            _mockUsuarioService.Setup(s => s.GetAll()).Returns(usuarios);

            // Act
            var result = _usuarioController.Get();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(usuarios, okResult.Value);
        }

        [Fact]
        public void GetAll_ShouldReturnBadRequest_WhenServiceThrowsException()
        {
            // Arrange
            _mockUsuarioService.Setup(s => s.GetAll()).Throws(new Exception("Erro inesperado"));

            // Act
            var result = _usuarioController.Get();

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var value = badRequest.Value?.GetType().GetProperty("mensagem");
            Assert.NotNull(value);
            Assert.Equal("Erro inesperado", value.GetValue(badRequest.Value)?.ToString());
        }

        [Fact]
        public void GetById_ShouldReturnOkWithUsuario()
        {
            // Arrange
            var usuario = new UsuarioDTO
            {
                Id = 1,
                Nome = "Yuri Santiago",
                Email = "yuri@email.com",
                Senha = Base64Helper.Encode("yuri"),
                Role = "ADMIN"
            };

            _mockUsuarioService.Setup(s => s.GetById(1)).Returns(usuario);

            // Act
            var result = _usuarioController.Get(1);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, ok.StatusCode);
            Assert.Equal(usuario, ok.Value);
        }

        [Fact]
        public void GetById_ShouldReturnBadRequest_WhenServiceThrowsException()
        {
            // Arrange
            _mockUsuarioService.Setup(s => s.GetById(It.IsAny<int>())).Throws(new Exception("Erro ao buscar ID"));

            // Act
            var result = _usuarioController.Get(1);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var value = badRequest.Value?.GetType().GetProperty("mensagem");
            Assert.NotNull(value);
            Assert.Equal("Erro ao buscar ID", value.GetValue(badRequest.Value)?.ToString());
        }

        [Fact]
        public void GetByRole_ShouldReturnOkWithUsuarios()
        {
            // Arrange
            var usuarios = new List<UsuarioDTO>
            {
               new() {
                Id = 1,
                Nome = "Yuri Santiago",
                Email = "yuri@email.com",
                Senha = Base64Helper.Encode("yuri"),
                Role = "ADMIN"
               }
             };

            _mockUsuarioService.Setup(s => s.GetAllByRole("ADMIN")).Returns(usuarios);

            // Act
            var result = _usuarioController.Get("ADMIN");

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, ok.StatusCode);
            Assert.Equal(usuarios, ok.Value);
        }

        [Fact]
        public void GetByRole_ShouldReturnBadRequest_WhenServiceThrowsException()
        {
            // Arrange
            _mockUsuarioService.Setup(s => s.GetAllByRole(It.IsAny<string>())).Throws(new Exception("Erro ao buscar por role"));

            // Act
            var result = _usuarioController.Get("ADMIN");

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var value = badRequest.Value?.GetType().GetProperty("mensagem");
            Assert.NotNull(value);
            Assert.Equal("Erro ao buscar por role", value.GetValue(badRequest.Value)?.ToString());
        }

        [Fact]
        public async Task Post_ShouldReturnOk_WhenUsuarioIsValidAndEmailNotExists()
        {
            // Arrange
            var usuarioRequest = new UsuarioRequest()
            {
                Nome = "Yuri Santiago",
                Email = "yuri@email.com",
                Senha = Base64Helper.Encode("yuri"),
                Role = "ADMIN"
            };

            var endpointMock = new Mock<ISendEndpoint>();
            _mockUsuarioService.Setup(s => s.GetAll()).Returns([]);
            _mockBus.Setup(b => b.GetSendEndpoint(It.IsAny<Uri>())).ReturnsAsync(endpointMock.Object);
            _mockConfiguration.Setup(c => c.GetSection("MassTransit:Queues")["UsuarioCadastroQueue"]).Returns("filaCadastroUsuario");

            // Act
            var result = await _usuarioController.Post(usuarioRequest);

            // Assert
            var ok = Assert.IsType<OkResult>(result);
            endpointMock.Verify(e => e.Send(usuarioRequest, default), Times.Once);
        }

        [Fact]
        public async Task Post_ShouldReturnBadRequest_WhenQueueFails()
        {
            // Arrange
            var usuarioRequest = new UsuarioRequest()
            {
                Nome = "Yuri Santiago",
                Email = "yuri@email.com",
                Senha = Base64Helper.Encode("yuri"),
                Role = "ADMIN"
            };

            _mockUsuarioService.Setup(s => s.GetAll()).Returns(new List<UsuarioDTO>());
            _mockConfiguration.Setup(c => c.GetSection("MassTransit:Queues")["UsuarioCadastroQueue"]).Returns("filaCadastroUsuario");
            _mockBus.Setup(b => b.GetSendEndpoint(It.IsAny<Uri>())).ThrowsAsync(new Exception("Falha no RabbitMQ"));

            // Act
            var result = await _usuarioController.Post(usuarioRequest);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var value = badRequest.Value?.GetType().GetProperty("mensagem");
            Assert.NotNull(value);
            Assert.Equal("Falha no RabbitMQ", value.GetValue(badRequest.Value)?.ToString());
        }

        [Fact]
        public async Task Put_ShouldReturnOk_WhenUpdateRequestIsValid()
        {
            // Arrange
            var usuarioUpdateRequest = new UsuarioUpdateRequest
            {
                Id = 1,
                Nome = "Yuri Santiago",
                Email = "yuri@email.com",
                Senha = Base64Helper.Encode("yuri"),
                Role = "ADMIN"
            };

            var endpointMock = new Mock<ISendEndpoint>();
            _mockBus.Setup(b => b.GetSendEndpoint(It.IsAny<Uri>())).ReturnsAsync(endpointMock.Object);
            _mockConfiguration.Setup(c => c.GetSection("MassTransit:Queues")["UsuarioAtualizacaoQueue"]).Returns("filaAtualizacaoUsuario");

            // Act
            var result = await _usuarioController.Put(usuarioUpdateRequest);

            // Assert
            var ok = Assert.IsType<OkResult>(result);
            endpointMock.Verify(e => e.Send(usuarioUpdateRequest, default), Times.Once);
        }

        [Fact]
        public async Task Put_ShouldReturnBadRequest_WhenQueueFails()
        {
            // Arrange
            var usuarioUpdateRequest = new UsuarioUpdateRequest
            {
                Id = 1,
                Nome = "Yuri Santiago",
                Email = "yuri@email.com",
                Senha = Base64Helper.Encode("yuri"),
                Role = "ADMIN"
            };

            _mockConfiguration.Setup(c => c.GetSection("MassTransit:Queues")["UsuarioAtualizacaoQueue"]).Returns("filaAtualizacaoUsuario");

            _mockBus.Setup(b => b.GetSendEndpoint(It.IsAny<Uri>())).ThrowsAsync(new Exception("Falha na fila"));

            // Act
            var result = await _usuarioController.Put(usuarioUpdateRequest);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var value = badRequest.Value?.GetType().GetProperty("mensagem");
            Assert.NotNull(value);
            Assert.Equal("Falha na fila", value.GetValue(badRequest.Value)?.ToString());
        }

        [Fact]
        public async Task Delete_ShouldReturnOk_WhenIdIsValid()
        {
            // Arrange
            int id = 1;

            var endpointMock = new Mock<ISendEndpoint>();
            _mockBus.Setup(b => b.GetSendEndpoint(It.IsAny<Uri>())).ReturnsAsync(endpointMock.Object);
            _mockConfiguration.Setup(c => c.GetSection("MassTransit:Queues")["UsuarioExclusaoQueue"]).Returns("filaExclusaoUsuario");

            // Act
            var result = await _usuarioController.Delete(new UsuarioDeleteRequest { Id = id });

            // Assert
            var ok = Assert.IsType<OkResult>(result);
            endpointMock.Verify(e => e.Send(It.Is<UsuarioDeleteRequest>(r => r.Id == id), default), Times.Once);
        }

        [Fact]
        public async Task Delete_ShouldReturnBadRequest_WhenQueueFails()
        {
            // Arrange
            int id = 1;

            _mockConfiguration.Setup(c => c.GetSection("MassTransit:Queues")["UsuarioExclusaoQueue"]).Returns("usuario-exclusao-queue");
            _mockBus.Setup(b => b.GetSendEndpoint(It.IsAny<Uri>())).ThrowsAsync(new Exception("Falha ao deletar"));

            // Act
            var result = await _usuarioController.Delete(new UsuarioDeleteRequest { Id = id});

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var value = badRequest.Value?.GetType().GetProperty("mensagem");
            Assert.NotNull(value);
            Assert.Equal("Falha ao deletar", value.GetValue(badRequest.Value)?.ToString());
        }

    }
}
