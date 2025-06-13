using Core.Interfaces.Services;
using Core.Requests.Update;
using MassTransit;

namespace ProdutoConsumidor.Eventos
{
    public class UsuarioAtualizado : IConsumer<UsuarioUpdateRequest>
    {

        private readonly IUsuarioService _usuarioService;

        public UsuarioAtualizado(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        public Task Consume(ConsumeContext<UsuarioUpdateRequest> context)
        {
            _usuarioService.Put(context.Message);
            return Task.CompletedTask;
        }

    }
}
