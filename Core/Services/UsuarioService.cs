using Core.DTOs;
using Core.Entities;
using Core.Helper;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Core.Requests.Create;
using Core.Requests.Update;

namespace Core.Services
{
    public class UsuarioService : IUsuarioService
    {

        private readonly IUsuarioRepository _usuarioRepository;

        public UsuarioService(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public IList<UsuarioDTO> GetAll()
        {
            var usuariosDTO = new List<UsuarioDTO>();
            var usuarios = _usuarioRepository.GetAll();

            foreach (var usuario in usuarios)
            {
                usuariosDTO.Add(new UsuarioDTO()
                {
                    Id = usuario.Id,
                    DataInclusao = usuario.DataInclusao,
                    Nome = usuario.Nome,
                    Email = usuario.Email,
                    Senha = usuario.Senha,
                    Role = usuario.Role
                });
            }

            return usuariosDTO;
        }

        public UsuarioDTO GetById(int id)
        {
            var usuario = _usuarioRepository.GetById(id);

            var usuarioDTO = new UsuarioDTO()
            {
                Id = usuario.Id,
                DataInclusao = usuario.DataInclusao,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Senha = usuario.Senha,
                Role = usuario.Role
            };

            return usuarioDTO;
        }

        public IList<UsuarioDTO> GetAllByRole(string role)
        {
            var usuariosDTO = new List<UsuarioDTO>();
            var usuarios = _usuarioRepository.GetAllByRole(role);

            foreach (var usuario in usuarios)
            {
                usuariosDTO.Add(new UsuarioDTO()
                {
                    Id = usuario.Id,
                    DataInclusao = usuario.DataInclusao,
                    Nome = usuario.Nome,
                    Email = usuario.Email,
                    Senha = usuario.Senha,
                    Role = usuario.Role
                });
            }

            return usuariosDTO;
        }

        public void Create(UsuarioRequest usuarioRequest)
        {

            var usuario = new Usuario()
            {
                Nome = usuarioRequest.Nome,
                Email = usuarioRequest.Email,
                Senha = Base64Helper.Encode(usuarioRequest.Senha),
                Role = usuarioRequest.Role
            };

            _usuarioRepository.Create(usuario);
        }

        public void Put(UsuarioUpdateRequest usuarioUpdateRequest)
        {
            var usuario = _usuarioRepository.GetById(usuarioUpdateRequest.Id);

            usuario.Nome = usuarioUpdateRequest.Nome ?? usuario.Nome;
            usuario.Email = usuarioUpdateRequest.Email ?? usuario.Email;
            usuario.Senha = usuarioUpdateRequest.Senha is not null ? Base64Helper.Encode(usuarioUpdateRequest.Senha) : usuario.Senha;
            usuario.Role = usuarioUpdateRequest.Role ?? usuario.Role;

            _usuarioRepository.Update(usuario);
        }

        public void Delete(int id)
        {
            _usuarioRepository.Delete(id);
        }

    }
}
