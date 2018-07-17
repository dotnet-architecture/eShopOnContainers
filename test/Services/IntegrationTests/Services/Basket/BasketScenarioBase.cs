using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System.IO;

namespace IntegrationTests.Services.Basket
{
    public class BasketScenarioBase
    {
        private const string ApiUrlBase = "api/v1/basket";

        public TestServer CreateServer()
        {
            var webHostBuilder = WebHost.CreateDefaultBuilder();
            webHostBuilder.UseContentRoot(Directory.GetCurrentDirectory() + "\\Services\\basket");
            webHostBuilder.UseStartup<BasketTestsStartup>();           

            return new TestServer(webHostBuilder);
        }

        public static class Get
        {
            public static string GetBasket(int id)
            {
                return $"{ApiUrlBase}/{id}";
            }
        }

        public static class Post
        {
            public static string Basket = $"{ApiUrlBase}/";
            public static string CheckoutOrder = $"{ApiUrlBase}/checkout";
        }
    }
}
