using Microsoft.AspNetCore.Mvc.Testing;

namespace Catalog.FunctionalTests;

public class CatalogScenariosBase 
{
    public TestServer CreateServer()
    {
        var factory = new CatalogApplication();
        return factory.CreateServer();
    }

    private class CatalogApplication : WebApplicationFactory<Program>
    {
        public TestServer CreateServer()
        {
            Services
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

            return Server;
        }

        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.ConfigureAppConfiguration(c =>
            {
                var directory = Path.GetDirectoryName(typeof(CatalogScenariosBase).Assembly.Location)!;

                c.AddJsonFile(Path.Combine(directory, "appsettings.json"), optional: false);
            });

            return base.CreateHost(builder);
        }
    }

    public static class Get
    {
        private const int PageIndex = 0;
        private const int PageCount = 4;

        public static string Items(bool paginated = false)
        {
            return paginated
                ? "api/v1/catalog/items" + Paginated(PageIndex, PageCount)
                : "api/v1/catalog/items";
        }

        public static string ItemById(int id)
        {
            return $"api/v1/catalog/items/{id}";
        }

        public static string ItemByName(string name, bool paginated = false)
        {
            return paginated
                ? $"api/v1/catalog/items/withname/{name}" + Paginated(PageIndex, PageCount)
                : $"api/v1/catalog/items/withname/{name}";
        }

        public static string Types = "api/v1/catalog/catalogtypes";

        public static string Brands = "api/v1/catalog/catalogbrands";

        public static string Filtered(int catalogTypeId, int catalogBrandId, bool paginated = false)
        {
            return paginated
                ? $"api/v1/catalog/items/type/{catalogTypeId}/brand/{catalogBrandId}" + Paginated(PageIndex, PageCount)
                : $"api/v1/catalog/items/type/{catalogTypeId}/brand/{catalogBrandId}";
        }

        private static string Paginated(int pageIndex, int pageCount)
        {
            return $"?pageIndex={pageIndex}&pageSize={pageCount}";
        }
    }
}
