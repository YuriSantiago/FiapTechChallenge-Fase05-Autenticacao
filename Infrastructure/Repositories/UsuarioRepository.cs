using Core.Entities;
using Core.Interfaces.Repositories;

namespace Infrastructure.Repositories
{
    public class UsuarioRepository : BaseRepository<Usuario>, IUsuarioRepository
    {

        public UsuarioRepository(ApplicationDbContext context) : base(context)
        {

        }

        public IList<Usuario> GetAllByRole(string role)
        {
            return [.. _context.Usuarios.Where(r => r.Role == role)];
        }

    }
}
