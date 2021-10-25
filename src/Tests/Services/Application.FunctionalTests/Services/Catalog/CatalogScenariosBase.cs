﻿namespace FunctionalTests.Services.Catalog;
using Microsoft.eShopOnContainers.Services.Catalog.API;

public class CatalogScenariosBase
{
    public TestServer CreateServer()
    {
        var path = Assembly.GetAssembly(typeof(CatalogScenariosBase))
            .Location;

        var hostBuilder = new WebHostBuilder()
            .UseContentRoot(Path.GetDirectoryName(path))
            .ConfigureAppConfiguration(cb =>
            {
                cb.AddJsonFile("Services/Catalog/appsettings.json", optional: false)
                .AddEnvironmentVariables();
            }).UseStartup<Startup>();

        var testServer = new TestServer(hostBuilder);

        testServer.Host
            .MigrateDbContext<CatalogContext>((context, services) =>
            {
                var env = services.GetService<IWebHostEnvironment>();
                var settings = services.GetService<IOptions<CatalogSettings>>();
                var logger = services.GetService<ILogger<CatalogContextSeed>>();

                new CatalogContextSeed()
                .SeedAsync(context, env, settings, logger)
                .Wait();
            })
            .MigrateDbContext<IntegrationEventLogContext>((_, __) => { });

        return testServer;
    }

    public static class Get
    {
        public static string Orders = "api/v1/orders";

        public static string Items = "api/v1/catalog/items";

        public static string ProductByName(string name)
        {
            return $"api/v1/catalog/items/withname/{name}";
        }
    }

    public static class Put
    {
        public static string UpdateCatalogProduct = "api/v1/catalog/items";
    }
}
