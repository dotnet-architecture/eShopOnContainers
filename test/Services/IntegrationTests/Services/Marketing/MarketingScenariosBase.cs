namespace IntegrationTests.Services.Marketing
{
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.TestHost;
    using Microsoft.eShopOnContainers.Services.Marketing.API.Infrastructure;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.DependencyInjection;
    using System.IO;

    public class MarketingScenarioBase
    {
        public static string CampaignsUrlBase => "api/v1/campaigns";

        public TestServer CreateServer()
        {
            var webHostBuilder = WebHost.CreateDefaultBuilder();
            webHostBuilder.UseContentRoot(Directory.GetCurrentDirectory() + "\\Services\\Marketing");
            webHostBuilder.UseStartup<MarketingTestsStartup>();

            var testServer = new TestServer(webHostBuilder);

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