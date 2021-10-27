using BusinessLogic.Data;
using Core.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var loggerFactory = services.GetRequiredService<ILoggerFactory>();

                try
                {
                    // crear base de datos de data de la aplicación(marketplace)
                    var context = services.GetRequiredService<MarketDbContext>();
                    await context.Database.MigrateAsync();
                    await MarketDbContextData.CargarDataAsync(context, loggerFactory);

                    // crear base de datos de seguridad(identitySeguridad)
                    var userManager = services.GetRequiredService<UserManager<Usuario>>();
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                    var identityContext = services.GetRequiredService<SeguridadDbContext>();
                    await identityContext.Database.MigrateAsync();
                    // crear usuario y rol inicial
                    await SeguridadDbContextData.SeedUserAsync(userManager, roleManager);
                }
                catch (Exception e)
                {
                    var logger = loggerFactory.CreateLogger<Program>();
                    logger.LogError(e, "Errores en el proceso de migración");
                }
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
