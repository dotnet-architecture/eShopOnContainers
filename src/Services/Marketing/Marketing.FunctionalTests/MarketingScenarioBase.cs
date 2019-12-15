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
            var path = Assembly.GetAssembly(typeof(MarketingScenarioBase))
                .Location;

            var hostBuilder = new WebHostBuilder()
                .UseContentRoot(Path.GetDirectoryName(path))
                .ConfigureAppConfiguration(cb =>
                {
                    cb.AddJsonFile("appsettings.json", optional: false)
                      .AddEnvironmentVariables();
                })
                .CaptureStartupErrors(true)
                .UseStartup<MarketingTestsStartup>();

            var testServer =  new TestServer(hostBuilder);

            testServer.Host
               .MigrateDbContext<MarketingContext>((context, services) =>
               {
                   var logger = services.GetService<ILogger<MarketingContextSeed>>();

                   new MarketingContextSeed()
                       .SeedAsync(context, logger)
                       .Wait();
               });

            return testServer;
        }
    }
}
