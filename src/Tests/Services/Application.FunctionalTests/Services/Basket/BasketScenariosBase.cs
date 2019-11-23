using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Reflection;

namespace FunctionalTests.Services.Basket
{
    public class BasketScenariosBase
    {
        private const string ApiUrlBase = "api/v1/basket";


        public TestServer CreateServer()
        {
            var path = Assembly.GetAssembly(typeof(BasketScenariosBase))
               .Location;

            var hostBuilder = new WebHostBuilder()
                .UseContentRoot(Path.GetDirectoryName(path))
                .ConfigureAppConfiguration(cb =>
                {
                    cb.AddJsonFile("Services/Basket/appsettings.json", optional: false)
                    .AddEnvironmentVariables();
                }).UseStartup<BasketTestsStartup>();

            return new TestServer(hostBuilder);
        }

        public static class Get
        {
            public static string GetBasket(int id)
            {
                return $"{ApiUrlBase}/{id}";
            }

            public static string GetBasketByCustomer(string customerId)
            {
                return $"{ApiUrlBase}/{customerId}";
            }
        }

        public static class Post
        {
            public static string CreateBasket = $"{ApiUrlBase}/";
            public static string CheckoutOrder = $"{ApiUrlBase}/checkout";
        }
    }
}
