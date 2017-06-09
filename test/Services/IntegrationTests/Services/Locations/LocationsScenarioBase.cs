using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace IntegrationTests.Services.Locations
{
    public class LocationsScenarioBase
    {
        public TestServer CreateServer()
        {
            var webHostBuilder = new WebHostBuilder();
            webHostBuilder.UseContentRoot(Directory.GetCurrentDirectory() + "\\Services\\Locations");
            webHostBuilder.UseStartup<LocationsTestsStartup>();

            return new TestServer(webHostBuilder);
        }

        public static class Get
        {
            public static string Locations = "api/v1/locations";

            public static string LocationBy(string id)
            {
                return $"api/v1/locations/{id}";
            }

            public static string UserLocationBy(int id)
            {
                return $"api/v1/locations/user/{id}";
            }
        }

        public static class Post
        {
            public static string AddNewLocation = "api/v1/locations/";
        }
    }
}
