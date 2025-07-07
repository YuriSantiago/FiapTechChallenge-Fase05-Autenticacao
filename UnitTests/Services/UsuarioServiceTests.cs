using Core.Entities;
using Core.Helper;
using Core.Interfaces.Repositories;
using Core.Requests.Create;
using Core.Requests.Update;
using Core.Services;
using Moq;

namespace UnitTests.Services
{
    public class UsuarioServiceTests
    {

        private readonly Mock<IUsuarioRepository> _usuarioRepositoryMock;
        private readonly UsuarioService _usuarioService;

        public UsuarioServiceTests()
        {
            _usuarioRepositoryMock = new Mock<IUsuarioRepository>();
            _usuarioService = new UsuarioService(_usuarioRepositoryMock.Object);
        }

        [Fact]
        public void GetAll_ShouldReturnListOfUsuarioDTO()
        {
            // Arrange
            var usuarios = new List<Usuario>
            {
               new() {
                Id = 1,
                Nome = "Yuri Santiago",
                Email = "yuri@email.com",
                Senha = Base64Helper.Encode("yuri"),
                Role = "ADMIN"
               }
             };

            _usuarioRepositoryMock.Setup(repo => repo.GetAll()).Returns(usuarios);

            // Act
            var result = _usuarioService.GetAll();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(usuarios.Count, result.Count);
        }

        [Fact]
        public void GetById_ShouldReturnUsuarioDTO_WhenIdExists()
        {
            // Arrange
            var usuario = new Usuario
            {
                Id = 1,
                Nome = "Yuri Santiago",
                Email = "yuri@email.com",
                Senha = Base64Helper.Encode("yuri"),
                Role = "ADMIN"
            };

            _usuarioRepositoryMock.Setup(repo => repo.GetById(usuario.Id)).Returns(usuario);

            // Act
            var result = _usuarioService.GetById(usuario.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(usuario.Id, result.Id);
        }

        [Fact]
        public void GetAllByRole_ShouldReturnListOfUsuarioDTOByRole()
        {
            // Arrange
            var usuarios = new List<Usuario>
            {
               new() {
                Id = 1,
                Nome = "Yuri Santiago",
                Email = "yuri@email.com",
                Senha = Base64Helper.Encode("yuri"),
                Role = "ADMIN"
               }
             };

            _usuarioRepositoryMock.Setup(repo => repo.GetAllByRole("ADMIN")).Returns(usuarios);

            // Act
            var result = _usuarioService.GetAllByRole("ADMIN");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(usuarios.Count, result.Count);
        }

        [Fact]
        public void Create_ShouldCallRepository_WhenRequestValid()
        {
            // Arrange
            var usuarioRequest = new UsuarioRequest()
            {
                Nome = "Yuri Santiago",
                Email = "yuri@email.com",
                Senha = Base64Helper.Encode("yuri"),
                Role = "ADMIN"
            };

            // Act
            _usuarioService.Create(usuarioRequest);

            // Assert
            _usuarioRepositoryMock.Verify(repo => repo.Create(It.IsAny<Usuario>()), Times.Once);
        }

        [Fact]
        public void Put_ShouldUpdateUsuario_WhenUsuarioExists()
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

            var usuario = new Usuario
            {
                Id = 1,
                Nome = "Yuri Santiago",
                Email = "yuri@email.com",
                Senha = Base64Helper.Encode("yuri"),
                Role = "ADMIN"
            };

            _usuarioRepositoryMock.Setup(repo => repo.GetById(usuarioUpdateRequest.Id)).Returns(usuario);

            // Act
            _usuarioService.Put(usuarioUpdateRequest);

            // Assert
            _usuarioRepositoryMock.Verify(repo => repo.Update(It.IsAny<Usuario>()), Times.Once);
        }

        [Fact]
        public void Delete_ShouldCallRepositoryDelete_WhenIdExists()
        {
            // Arrange
            var id = 1;

            // Act
            _usuarioService.Delete(id);

            // Assert
            _usuarioRepositoryMock.Verify(repo => repo.Delete(id), Times.Once);
        }

    }
}
