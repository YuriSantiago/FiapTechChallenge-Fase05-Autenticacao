using Core.DTOs;
using Core.Requests.Create;
using Core.Requests.Update;

namespace Core.Interfaces.Services
{
    public interface IUsuarioService
    {

        IList<UsuarioDTO> GetAll();

        UsuarioDTO GetById(int id);

        IList<UsuarioDTO> GetAllByRole(string role);

        void Create(UsuarioRequest produtoRequest);

        void Put(UsuarioUpdateRequest produtoUpdateRequest);

        void Delete(int id);

    }
}
