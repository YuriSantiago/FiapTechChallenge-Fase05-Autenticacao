using Core.DTOs;
using Core.Requests.Create;

namespace Core.Interfaces.Services
{
    public interface IAuthService
    {

        LoginDTO GetToken(LoginRequest loginRequest);

    }
}
