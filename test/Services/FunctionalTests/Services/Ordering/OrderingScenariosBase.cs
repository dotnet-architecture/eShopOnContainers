using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FunctionalTests.Services.Ordering
{
    public class OrderingScenariosBase
    {
        public TestServer CreateServer()
        {
            var webHostBuilder = new WebHostBuilder();
            webHostBuilder.UseContentRoot(Directory.GetCurrentDirectory() + "\\Services\\Ordering");
            webHostBuilder.UseStartup<OrderingTestsStartup>();

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
