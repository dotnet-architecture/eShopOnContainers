namespace FunctionalTests.Services.Ordering
{
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.TestHost;
    using Microsoft.eShopOnContainers.Services.Ordering.API;
    using System.IO;

    public class OrderingScenarioBase
    {
        public TestServer CreateServer()
        {
            var webHostBuilder = new WebHostBuilder();
            webHostBuilder.UseContentRoot(Directory.GetCurrentDirectory());
            webHostBuilder.UseStartup<Startup>();

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

        public static class Post
        {
            public static string AddNewOrder = "api/v1/orders/new";
        }
    }
}
