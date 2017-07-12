namespace FunctionalTests.Services.Basket
{
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.TestHost;
    using System.IO;

    public class BasketScenariosBase
    {
        private const string ApiUrlBase = "api/v1/basket";

        public TestServer CreateServer()
        {
            var webHostBuilder = new WebHostBuilder();
            webHostBuilder.UseContentRoot(Directory.GetCurrentDirectory());
            webHostBuilder.UseStartup<BasketTestsStartup>();

            return new TestServer(webHostBuilder);
        }

        public static class Get
        {
            public static string GetBasketByCustomer(string customerId)
            {
                return $"{ApiUrlBase}/{customerId}";
            }
        }

        public static class Post
        {
            public static string CreateBasket = $"{ApiUrlBase}/";
            public static string Checkout = $"{ApiUrlBase}/checkout";
        }
    }
}