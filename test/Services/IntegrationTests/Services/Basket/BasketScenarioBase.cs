using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.eShopOnContainers.Services.Basket.API;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace IntegrationTests.Services.Basket
{
    public class BasketScenarioBase
    {
        public TestServer CreateServer()
        {
            var webHostBuilder = new WebHostBuilder();
            webHostBuilder.UseContentRoot(Directory.GetCurrentDirectory() + "\\Services\\basket");
            webHostBuilder.UseStartup<BasketTestsStartup>();

            return new TestServer(webHostBuilder);
        }

        public static class Get
        {
            public static string GetBasket(int id)
            {
                return $"api/v1/basket/{id}";
            }
        }

        public static class Post
        {
            public static string Basket = "api/v1/basket";
            public static string CheckoutOrder = "api/v1/basket/checkout";
        }
    }
}
