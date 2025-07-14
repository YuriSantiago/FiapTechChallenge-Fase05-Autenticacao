using Core.DTOs;
using Core.Helper;
using Core.Requests.Create;
using Core.Requests.Delete;
using Core.Requests.Update;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace IntegrationTests.Cadastro
{
    public class UsuarioControllerTests : IClassFixture<CustomWebApplicationFactory<UsuarioProdutor.Program>>
    {
        private readonly HttpClient _client;

        public UsuarioControllerTests(CustomWebApplicationFactory<UsuarioProdutor.Program> factory)
        {
            _client = factory.CreateClient();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("TestScheme");
        }

        [Fact]
        public async Task GetAll_ShouldReturnOk()
        {
            // Act
            var response = await _client.GetAsync("/Usuario");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var usuarios = await response.Content.ReadFromJsonAsync<List<UsuarioDTO>>();
            Assert.NotNull(usuarios);
            Assert.True(usuarios.Count >= 0);
        }

        [Fact]
        public async Task GetById_ShouldReturnUsuario_WhenIdExists()
        {
            // Arrange
            int usuarioId = 1;

            // Act
            var response = await _client.GetAsync($"/Usuario/{usuarioId}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var usuario = await response.Content.ReadFromJsonAsync<UsuarioDTO>();
            Assert.NotNull(usuario);
            Assert.Equal(usuarioId, usuario.Id);
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFound_WhenIdDoesNotExist()
        {
            // Arrange
            int usuarioId = 9999;

            // Act
            var response = await _client.GetAsync($"/Usuario/{usuarioId}");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GetAllByRole_ShouldReturnOk_WhenRoleExists()
        {
            // Arrange
            string role = "CLIENTE";

            // Act
            var response = await _client.GetAsync($"/Usuario/getAllByRole/{role}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var usuarios = await response.Content.ReadFromJsonAsync<List<UsuarioDTO>>();
            Assert.NotNull(usuarios);
            Assert.True(usuarios.Count >= 0);
        }

        [Fact]
        public async Task GetAllByRole_ShouldReturnNotResults_WhenRoleDoesNotExist()
        {
            // Arrange
            string role = "INEXISTENTE";

            // Act
            var response = await _client.GetAsync($"/Usuario/getAllByRole/{role}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var usuarios = await response.Content.ReadFromJsonAsync<List<UsuarioDTO>>();
            Assert.Equal(0, usuarios?.Count);
        }

        [Fact]
        public async Task Create_ShouldReturnOk_WhenContatoIsValid()
        {
            // Arrange
            var usuarioRequest = new UsuarioRequest
            {
                Nome = "Yago Santiago",
                Email = "yago@email.com",
                Senha = Base64Helper.Encode("yago"),
                Role = "FUNCIONARIO"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/Usuario", usuarioRequest);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Create_ShouldReturnBadRequest_WhenUsuarioIsInvalid()
        {
            // Arrange
            var usuarioRequest = new UsuarioRequest
            {
                Nome = "",
                Email = "yuri@email.com",
                Senha = Base64Helper.Encode("yuri"),
                Role = "ADMIN"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/Usuario", usuarioRequest);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Update_ShouldReturnOK_WhenUsuarioIsValid()
        {
            // Arrange
            var usuarioUpdateRequest = new UsuarioUpdateRequest
            {
                Id = 1,
                Email = "yuri@email.com",
                Senha = Base64Helper.Encode("yuri"),
                Role = "ADMIN"
            };

            // Act
            var response = await _client.PutAsJsonAsync("/Usuario", usuarioUpdateRequest);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Update_ShouldReturnOK_WhenUsuarioIsInvalid()
        {
            // Arrange
            var usuarioUpdateRequest = new UsuarioUpdateRequest
            {
                Id = 0,
                Email = "yuri@email.com",
                Senha = Base64Helper.Encode("yuri"),
                Role = "ADMIN"
            };

            // Act
            var response = await _client.PutAsJsonAsync("/Usuario", usuarioUpdateRequest);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Delete_ShouldReturnOk_WhenIdExists()
        {
            // Arrange
            var usuarioDeleteRequest = new HttpRequestMessage(HttpMethod.Delete, "/Usuario")
            {
                Content = JsonContent.Create(new UsuarioDeleteRequest{Id = 1})
            };

            // Act
            var response = await _client.SendAsync(usuarioDeleteRequest);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

    }
}
