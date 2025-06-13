using Core.Interfaces.Services;
using Core.Requests.Delete;
using MassTransit;

namespace ProdutoConsumidor.Eventos
{
    public class UsuarioDeletado : IConsumer<UsuarioDeleteRequest>
    {
        private readonly IUsuarioService _usuarioService;

        public UsuarioDeletado(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        public Task Consume(ConsumeContext<UsuarioDeleteRequest> context)
        {
            _usuarioService.Delete(context.Message.Id);
            return Task.CompletedTask;
        }
    }
}
