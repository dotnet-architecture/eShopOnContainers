using Autofac.Extensions.DependencyInjection;
using Catalog.API.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.eShopOnContainers.BuildingBlocks.IntegrationEventLogEF;
using Microsoft.eShopOnContainers.Services.Catalog.API;
using Microsoft.eShopOnContainers.Services.Catalog.API.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.IO;
using System.Reflection;

namespace Catalog.FunctionalTests
{
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
                    cb.AddJsonFile("appsettings.json", optional: false)
                    .AddEnvironmentVariables();
                })
                .UseStartup<Startup>();


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
}
