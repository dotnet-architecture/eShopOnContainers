using FunctionalTests.Middleware;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.eShopOnContainers.Services.Catalog.API;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FunctionalTests.Services.Catalog
{
    public class CatalogScenariosBase
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
            public static string Orders = "api/v1/orders";

            public static string Items = "api/v1/catalog/items";

            public static string ProductByName(string name)
            {
                return $"api/v1/catalog/items/withname/{name}";
            }
        }

        public static class Post
        {
            public static string UpdateCatalogProduct = "api/v1/catalog/update";
        }
    }
}
