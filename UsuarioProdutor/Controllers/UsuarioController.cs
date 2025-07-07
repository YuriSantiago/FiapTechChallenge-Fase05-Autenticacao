using Core.Interfaces.Services;
using Core.Requests.Create;
using Core.Requests.Delete;
using Core.Requests.Update;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace UsuarioProdutor.Controllers
{
    [ApiController]
    [Route("/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly IBus _bus;
        private readonly IConfiguration _configuration;
        private readonly IUsuarioService _usuarioService;

        public UsuarioController(IBus bus, IConfiguration configuration, IUsuarioService usuarioService)
        {
            _bus = bus;
            _configuration = configuration;
            _usuarioService = usuarioService;
        }

        /// <summary>
        /// Busca todos os usuários cadastrados
        /// </summary>
        /// <returns>Retorna todos os usuários cadastrados</returns>
        /// <response code="200">Listagem retornada com sucesso</response>
        /// <response code="400">Erro ao listar os usuários</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [HttpGet]
        [Authorize(Roles = "ADMIN")]
        public IActionResult Get()
        {
            try
            {
                return Ok(_usuarioService.GetAll());
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        /// <summary>
        /// Busca um usuário por Id
        /// </summary>
        /// <param name="id">Id do Usuário</param>
        /// <returns>Retorna um usuário filtrado por um Id</returns>
        /// <response code="200">Usuário retornado com sucesso</response>
        /// <response code="400">Erro ao buscar o usuário</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [HttpGet("{id:int}")]
        [Authorize(Roles = "ADMIN")]
        public IActionResult Get([FromRoute] int id)
        {
            try
            {
                return Ok(_usuarioService.GetById(id));
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        /// <summary>
        /// Busca usuários por role
        /// </summary>
        /// <param name="role">Role dos usuários</param>
        /// <returns>Retorna uma lista de usuários filtrada por uma role</returns>
        /// <response code="200">Usuários retornados com sucesso</response>
        /// <response code="400">Erro ao buscar a lista de usuários por role</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [HttpGet("getAllByRole/{role}")]
        [Authorize(Roles = "ADMIN")]
        public IActionResult Get([FromRoute] string role)
        {
            try
            {
                return Ok(_usuarioService.GetAllByRole(role));
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        /// <summary>
        /// Cadastra um novo usuário 
        /// </summary>
        /// <param name="usuarioRequest">Objeto do tipo "UsuarioRequest"</param>
        /// <response code="200">Usuário cadastrado com sucesso</response>
        /// <response code="400">Erro ao cadastrar o usuário</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Post([FromBody] UsuarioRequest usuarioRequest)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var usuarios = _usuarioService.GetAll().Any(x => x.Email == usuarioRequest.Email);

                if (usuarios)
                    return NotFound("E-mail já cadastrado");

                var nomeFila = _configuration.GetSection("MassTransit:Queues")["UsuarioCadastroQueue"] ?? string.Empty;
                var endpoint = await _bus.GetSendEndpoint(new Uri($"queue:{nomeFila}"));
                await endpoint.Send(usuarioRequest);

                return Ok();
            }

            catch (Exception ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        /// <summary>
        /// Atualiza as informações de um usuário
        /// </summary>
        /// <param name="usuarioUpdateRequest">Objeto do tipo "UsuarioUpdateRequest"</param>
        /// <response code="200">Usuário atualizado com sucesso</response>
        /// <response code="400">Erro ao atualizar o Usuário</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [HttpPut]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Put([FromBody] UsuarioUpdateRequest usuarioUpdateRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var nomeFila = _configuration.GetSection("MassTransit:Queues")["UsuarioAtualizacaoQueue"] ?? string.Empty;
                var endpoint = await _bus.GetSendEndpoint(new Uri($"queue:{nomeFila}"));
                await endpoint.Send(usuarioUpdateRequest);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        /// <summary>
        /// Deleta um usuário por Id
        /// </summary>
        /// <param name="id">Id do Usuário</param>
        /// <response code="200">Usuário deletado com sucesso</response>
        /// <response code="400">Erro ao deletar o usuário</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {

            try
            {
                var nomeFila = _configuration.GetSection("MassTransit:Queues")["UsuarioExclusaoQueue"] ?? string.Empty;
                var endpoint = await _bus.GetSendEndpoint(new Uri($"queue:{nomeFila}"));
                await endpoint.Send(new UsuarioDeleteRequest { Id = id });

                return Ok();
            }

            catch (Exception ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }


    }
}
