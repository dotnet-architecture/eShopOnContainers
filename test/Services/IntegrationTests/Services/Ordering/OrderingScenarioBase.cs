namespace IntegrationTests.Services.Ordering
{
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.TestHost;
    using Microsoft.eShopOnContainers.Services.Ordering.API;
    using Microsoft.Extensions.Configuration;
    using System.IO;

    public class OrderingScenarioBase
    {
        public TestServer CreateServer()
        {
            var webHostBuilder = WebHost.CreateDefaultBuilder();
            webHostBuilder.UseContentRoot(Directory.GetCurrentDirectory() + "\\Services\\Ordering");
            webHostBuilder.UseStartup<OrderingTestsStartup>();
            webHostBuilder.ConfigureAppConfiguration((builderContext, config) =>
            {
                config.AddJsonFile("settings.json");
            });

            return new TestServer(webHostBuilder);
        }

        public static class Get
        {
            public static string Orders = "api/v1/orders";

            public static string OrderBy(int id)
            {
                return $"api/v1/orders/{id}";
            }
        }

        public static class Put
        {
            public static string CancelOrder = "api/v1/orders/cancel";
            public static string ShipOrder = "api/v1/orders/ship";
        }
    }
}
