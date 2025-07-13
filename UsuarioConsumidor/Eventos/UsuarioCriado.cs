using Core.Interfaces.Services;
using Core.Requests.Create;
using MassTransit;

namespace UsuarioConsumidor.Eventos
{
    public class UsuarioCriado : IConsumer<UsuarioRequest>
    {

        private readonly IUsuarioService _usuarioService;

        public UsuarioCriado(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        public Task Consume(ConsumeContext<UsuarioRequest> context)
        {
            _usuarioService.Create(context.Message);
            return Task.CompletedTask;
        }

    }
}
