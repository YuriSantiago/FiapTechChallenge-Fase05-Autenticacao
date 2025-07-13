using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Core.Services;
using Infrastructure.Repositories;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using UsuarioConsumidor;
using UsuarioConsumidor.Eventos;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

var configuration = builder.Configuration;

var queueCadastroUsuario = configuration.GetSection("MassTransit:Queues")["UsuarioCadastroQueue"] ?? string.Empty;
var queueAtualizacaoUsuario = configuration.GetSection("MassTransit:Queues")["UsuarioAtualizacaoQueue"] ?? string.Empty;
var queueExclusaoUsuario = configuration.GetSection("MassTransit:Queues")["UsuarioExclusaoQueue"] ?? string.Empty;

builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(configuration.GetConnectionString("ConnectionString"),
        sql => sql.EnableRetryOnFailure());
}, ServiceLifetime.Scoped);

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<UsuarioCriado>();
    x.AddConsumer<UsuarioAtualizado>();
    x.AddConsumer<UsuarioDeletado>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(configuration.GetSection("MassTransit")["Server"], "/", h =>
        {
            h.Username(configuration.GetSection("MassTransit")["User"]);
            h.Password(configuration.GetSection("MassTransit")["Password"]);
        });

        cfg.ReceiveEndpoint(queueCadastroUsuario, e =>
        {
            e.ConfigureDefaultDeadLetterTransport();
            e.ConfigureConsumer<UsuarioCriado>(context);
        });

        cfg.ReceiveEndpoint(queueAtualizacaoUsuario, e =>
        {
            e.ConfigureDefaultDeadLetterTransport();
            e.ConfigureConsumer<UsuarioAtualizado>(context);
        });

        cfg.ReceiveEndpoint(queueExclusaoUsuario, e =>
        {
            e.ConfigureDefaultDeadLetterTransport();
            e.ConfigureConsumer<UsuarioDeletado>(context);
        });

        cfg.ConfigureEndpoints(context);
    });

   

});

var host = builder.Build();
host.Run();
