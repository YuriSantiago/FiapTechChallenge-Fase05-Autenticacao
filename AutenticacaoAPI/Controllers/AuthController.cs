using Core.Interfaces.Services;
using Core.Requests.Create;
using Microsoft.AspNetCore.Mvc;

namespace AutenticacaoAPI.Controllers
{

    [ApiController]
    [Route("/[controller]")]
    public class AuthController : ControllerBase
    {

        private readonly IAuthService _authService;
        private readonly IUsuarioService _usuarioService;

        public AuthController(IAuthService authService, IUsuarioService usuarioService)
        {
            _authService = authService;
            _usuarioService = usuarioService;
        }

        [HttpPost("getToken")]
        public IActionResult GetToken([FromBody] LoginRequest loginRequest)
        {
            try
            {
                var response = _authService.GetToken(loginRequest);
                return Ok(response);
            }
            catch (UnauthorizedAccessException e)
            {
                return Unauthorized(new { mensagem = e.Message });
            }
        }
    }
}
