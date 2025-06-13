using Core.DTOs;
using Core.Entities;

namespace Core.Interfaces.Repositories
{
    public interface IUsuarioRepository : IRepository<Usuario>
    {

        IList<Usuario> GetAllByRole(string role);

    }
}
