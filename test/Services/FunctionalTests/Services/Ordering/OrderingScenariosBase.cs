﻿using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
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

        public static class Post
        {
            public static string AddNewOrder = "api/v1/orders/new";
        }

        public static class Put
        {
            public static string CancelOrder = "api/v1/orders/cancel";
        }

        public static class Delete
        {
            public static string OrderBy(int id)
            {
                return $"api/v1/orders/{id}";
            }
        }
    }
}
