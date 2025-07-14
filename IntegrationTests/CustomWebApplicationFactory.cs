﻿using Core.Entities;
using Core.Helper;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace IntegrationTests
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                //builder.UseEnvironment("Testing");  

                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

                if (descriptor != null)
                    services.Remove(descriptor);

                services.RemoveAll<IConfigureOptions<AuthenticationOptions>>();
                services.RemoveAll<IConfigureOptions<Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerOptions>>();

                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDbForTesting");
                });

                services.AddAuthentication("TestScheme")
                  .AddScheme<AuthenticationSchemeOptions, JwtAuthHandlerSimulation>("TestScheme", options =>
                  {
                      options.TimeProvider = TimeProvider.System; 
                  });

                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                SeedDatabase(db).Wait();
            });

        }

        private static async Task SeedDatabase(ApplicationDbContext context)
        {

            context.Usuarios.RemoveRange(context.Usuarios);
            //context.Regioes.RemoveRange(context.Regioes);
            await context.SaveChangesAsync();

            //var regiaoSP = context.Regioes.Add(new Regiao
            //{
            //    DDD = 11,
            //    Descricao = "São Paulo",
            //    DataInclusao = DateTime.UtcNow
            //});

            //var regiaoRJ = context.Regioes.Add(new Regiao
            //{
            //    DDD = 21,
            //    Descricao = "Rio de Janeiro",
            //    DataInclusao = DateTime.UtcNow
            //});


            context.Usuarios.Add(new Usuario
            {
                //Id = 1,
                Nome = "Yuri Santiago",
                Email = "yuri@email.com",
                Senha = Base64Helper.Encode("yuri"),
                Role = "ADMIN"
            });

            //context.Contatos.Add(new Contato
            //{
            //    Nome = "Yago",
            //    Telefone = "999999999",
            //    Email = "yago@email.com",
            //    RegiaoId = 2,
            //    Regiao = regiaoRJ.Entity
            //});

            await context.SaveChangesAsync();
        }

    }
}