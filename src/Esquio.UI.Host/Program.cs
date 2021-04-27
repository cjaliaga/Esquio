using Esquio.UI.Host.Infrastructure.Data.Seed;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Esquio.UI.Host
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args)
                .Build()
                .MigrateDbContext(StoreDbContextSeed.Seed())
                .Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>()
                         .UseAzureAppConfiguration();
                })
                .UseSerilog((hostContext, services, loggerConfig) =>
                {
                    loggerConfig.ConfigureSinks(hostContext, services);
                });
    }
}
