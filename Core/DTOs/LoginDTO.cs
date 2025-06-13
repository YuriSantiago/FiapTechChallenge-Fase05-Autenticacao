namespace Core.DTOs
{
    public class LoginDTO
    {

        public required string Token { get; set; }

        public required string Role { get; set; }

        public required string Email { get; set; }

    }
}
