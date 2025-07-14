using Core.Requests.Create;
using System.Net;
using System.Net.Http.Json;

namespace IntegrationTests.Controller
{
    public class AuthControllerTests : IClassFixture<CustomWebApplicationFactory<AutenticacaoAPI.Program>>
    {

        private readonly HttpClient _client;

        public AuthControllerTests(CustomWebApplicationFactory<AutenticacaoAPI.Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetToken_ShouldReturnToken_WhenLoginIsValid()
        {
            // Arrange
            var request = new LoginRequest
            {
                Email = "yuri@email.com",
                Senha = "yuri" 
            };

            // Act
            var response = await _client.PostAsJsonAsync("/Auth/getToken", request);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetToken_ShouldReturnUnauthorized_WhenLoginIsInvalid()
        {
            // Arrange
            var request = new LoginRequest
            {
                Email = "admin@email.com",
                Senha = "senhaErrada"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/Auth/getToken", request);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

    }
}
