namespace Microsoft.eShopOnContainers.Services.Marketing.API
{
    using AspNetCore.Hosting;
    using Microsoft.AspNetCore;
    using Microsoft.eShopOnContainers.Services.Marketing.API.Infrastructure;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using System;
    using System.IO;

    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args)
            .MigrateDbContext<MarketingContext>((context, services) =>
            {
                var logger = services.GetService<ILogger<MarketingContextSeed>>();

                new MarketingContextSeed()
                    .SeedAsync(context,logger)
                    .Wait();

            }).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseApplicationInsights()
                .UseHealthChecks("/hc")
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .UseWebRoot("Pics")
                .ConfigureAppConfiguration((builderContext, config) =>
                {
                    var builtConfig = config.Build();

                    var configurationBuilder = new ConfigurationBuilder();

                    if (Convert.ToBoolean(builtConfig["UseVault"]))
                    {
                        configurationBuilder.AddAzureKeyVault(
                            $"https://{builtConfig["Vault:Name"]}.vault.azure.net/",
                            builtConfig["Vault:ClientId"],
                            builtConfig["Vault:ClientSecret"]);
                    }

                    configurationBuilder.AddEnvironmentVariables();

                    config.AddConfiguration(configurationBuilder.Build());
                })
                .ConfigureLogging((hostingContext, builder) =>
                {
                    builder.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    builder.AddConsole();
                    builder.AddDebug();
                })
                .UseApplicationInsights()
                .Build();
    }
}
