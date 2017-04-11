

namespace IntegrationTests.Services.Catalog
{
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.TestHost;
    using Microsoft.eShopOnContainers.Services.Catalog.API;
    using System.IO;

    public class CatalogScenarioBase
    {
        public TestServer CreateServer()
        {
            var webHostBuilder = new WebHostBuilder();
            webHostBuilder.UseContentRoot(Directory.GetCurrentDirectory() + "\\Services\\Catalog");
            webHostBuilder.UseStartup<Startup>();

            return new TestServer(webHostBuilder);
        }

        public static class Get
        {
            public static string Items = "api/v1/catalog/items";

            public static string Types = "api/v1/catalog/catalogtypes";

            public static string Brands = "api/v1/catalog/catalogbrands";

            public static string Paginated(int pageIndex, int pageCount)
            {
                return $"api/v1/catalog/items?pageIndex={pageIndex}&pageSize={pageCount}";
            }

            public static string Filtered(int catalogTypeId, int catalogBrandId)
            {
                return $"api/v1/catalog/items/type/{catalogTypeId}/brand/{catalogBrandId}";
            }
        }
    }
}
