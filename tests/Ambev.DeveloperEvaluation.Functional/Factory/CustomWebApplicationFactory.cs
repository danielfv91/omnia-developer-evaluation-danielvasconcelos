using Ambev.DeveloperEvaluation.Functional.Extensions;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.WebApi;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ambev.DeveloperEvaluation.Functional.Factory
{

    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        public string ConnectionString { get; set; } = default!;

        public CustomWebApplicationFactory(string connectionString)
        {
            ConnectionString = connectionString;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            Console.WriteLine("Configuring WebHost...");

            builder.ConfigureAppConfiguration((context, config) =>
            {
                config.AddEnvironmentVariables();
            });

            builder.ConfigureServices((context, services) =>
            {
                Console.WriteLine("Replacing DefaultContext with test container connection string...");
                ConnectionString = context.Configuration.GetConnectionString("DefaultConnection")!;

                services.RemoveDbContext<DefaultContext>();

                services.AddDbContext<DefaultContext>(options =>
                {
                    options.UseNpgsql(ConnectionString);
                });
            });
        }
    }
}