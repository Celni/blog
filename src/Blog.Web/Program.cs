using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Blog.Persistence;
using Blog.Web.Data;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Blog.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            MigrateContext<IdentityContext>(host);
            MigrateContext<BlogContext>(host);
            host.Run();
        }



        private static void MigrateContext<TContext>(IHost host) where TContext : DbContext
        {
            using var scope = host.Services.CreateScope();

            var services = scope.ServiceProvider;
            var logger = services.GetRequiredService<ILogger<Program>>();

            try
            {
                var context = services.GetRequiredService<TContext>();

                var connectionString = context.Database.GetDbConnection().ConnectionString;

                logger.LogInformation($"Connection string DB {typeof(TContext).Name}: {connectionString}");
                context.Database.EnsureCreated();
                context.Database.Migrate();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"An error occurred seeding the DB {typeof(TContext).Name}.");
            }

        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
