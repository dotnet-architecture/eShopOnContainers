using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.eShopOnContainers.Services.Marketing.API;
using Microsoft.eShopOnContainers.Services.Marketing.API.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Marketing.FunctionalTests
{
    public class MarketingScenarioBase
    {
        public static string CampaignsUrlBase => "api/v1/campaigns";

        public TestServer CreateServer()
        {
            
            Console.WriteLine(" Creating test server");
            var path = Assembly.GetAssembly(typeof(MarketingScenarioBase))
                .Location;

            Console.WriteLine(" Creating builder");

            var hostBuilder = new WebHostBuilder()
                .UseContentRoot(Path.GetDirectoryName(path))
                .ConfigureAppConfiguration(cb =>
                {
                    var h = cb.AddJsonFile("appsettings.json", optional: false)
                    .AddEnvironmentVariables();
                })
                .CaptureStartupErrors(true)
                .UseStartup<MarketingTestsStartup>();

            Console.WriteLine(" Created builder");
            
            var testServer =  new TestServer(hostBuilder);

              using (var scope = testServer.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                var logger = services.GetRequiredService<ILogger<MarketingScenarioBase>>();
                var settings = services.GetRequiredService<IOptions<MarketingSettings>>();
                logger.LogError("connectionString " + settings.Value.ConnectionString);
            Console.WriteLine("connectionString " + settings.Value.ConnectionString);
            }

            testServer.Host
               .RemoveDbContext<MarketingContext>()
               .MigrateDbContext<MarketingContext>((context, services) =>
               {
                   var logger = services.GetService<ILogger<MarketingContextSeed>>();

                    logger.LogError("Migrating MarketingContextSeed");
                   new MarketingContextSeed()
                       .SeedAsync(context, logger)
                       .Wait();

               });
            Console.WriteLine(" Thread to sleep");

            Thread.Sleep(5000);
            Console.WriteLine(" Thread after");


            return testServer;
        }
    }
}
