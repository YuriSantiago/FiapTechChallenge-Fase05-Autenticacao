using Core.DTOs;
using Core.Helpers;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Core.Requests.Create;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Core.Services
{
    public class AuthService : IAuthService
    {

        private readonly IConfiguration _configuration;
        private readonly IUsuarioRepository _usuarioRepository;

        public AuthService(IConfiguration configuration, IUsuarioRepository usuarioRepository)
        {
            _configuration = configuration;
            _usuarioRepository = usuarioRepository;
        }

        public LoginDTO GetToken(LoginRequest loginRequest)
        {
            var usuario = _usuarioRepository.GetAll().FirstOrDefault(x => x.Email == loginRequest.Email);

            if (usuario is null)
                throw new UnauthorizedAccessException("Usuário não encontrado");

            if (!usuario.Senha.Equals(Base64Helper.Encode(loginRequest.Senha)))
                throw new UnauthorizedAccessException("Senha Inválida");

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"] ?? string.Empty);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                [
                new Claim(ClaimTypes.Name, usuario.Email),
                new Claim(ClaimTypes.Role, usuario.Role)
            ]),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new LoginDTO
            {
                Token = tokenHandler.WriteToken(token),
                Role = usuario.Role,
                Email = usuario.Email
            };
        }

    }
}
