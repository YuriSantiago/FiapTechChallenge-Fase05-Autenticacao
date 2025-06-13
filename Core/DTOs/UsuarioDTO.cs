namespace Core.DTOs
{
    public class UsuarioDTO
    {
        public int Id { get; set; }

        public DateTime DataInclusao { get; set; }

        public required string Nome { get; set; }

        public required string Email { get; set; }

        public required string Senha { get; set; }

        public required string Role { get; set; }

    }
}
