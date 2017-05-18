using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FunctionalTests.Services.Basket
{
    public class BasketScenariosBase
    {
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
                return $"/{customerId}";
            }
        }

        public static class Post
        {
            public static string CreateBasket = "/";
            public static string Checkout = "/checkout";
        }
    }
}
