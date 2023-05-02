namespace FunctionalTests.Services.Catalog;

using FunctionalTests.Services.Ordering;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.eShopOnContainers.Services.Catalog.API;
using Microsoft.Extensions.Hosting;

public class CatalogScenariosBase : WebApplicationFactory<CatalogProgram>
{
    public TestServer CreateServer()
    {
        Services.MigrateDbContext<CatalogContext>((context, services) =>
        {
            var env = services.GetService<IWebHostEnvironment>();
            var settings = services.GetService<IOptions<CatalogSettings>>();
            var logger = services.GetService<ILogger<CatalogContextSeed>>();

            new CatalogContextSeed()
            .SeedAsync(context, env, settings, logger)
            .Wait();
        })
        .MigrateDbContext<IntegrationEventLogContext>((_, __) => { });

        return Server;
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(c =>
        {
            var directory = Path.GetDirectoryName(typeof(CatalogScenariosBase).Assembly.Location)!;

            c.AddJsonFile(Path.Combine(directory, "Services/Catalog/appsettings.json"), optional: false);
        });

        return base.CreateHost(builder);
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
