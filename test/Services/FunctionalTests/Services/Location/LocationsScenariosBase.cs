namespace FunctionalTests.Services.Locations
{
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.TestHost;
    using Microsoft.Extensions.Configuration;
    using System;
    using System.IO;

    public class LocationsScenariosBase
    {
        public TestServer CreateServer()
        {
            var webHostBuilder = WebHost.CreateDefaultBuilder();
            webHostBuilder.UseContentRoot(Directory.GetCurrentDirectory() + "\\Services\\Location");
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

            public static string UserLocationBy(Guid id)
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
