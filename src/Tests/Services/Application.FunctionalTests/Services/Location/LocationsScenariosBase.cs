namespace FunctionalTests.Services.Locations
{
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.TestHost;
    using Microsoft.Extensions.Configuration;
    using System;
    using System.IO;
    using System.Reflection;

    public class LocationsScenariosBase
    {
        public TestServer CreateServer()
        {
            var path = Assembly.GetAssembly(typeof(LocationsScenariosBase))
                .Location;

            var hostBuilder = new WebHostBuilder()
                .UseContentRoot(Path.GetDirectoryName(path))
                .ConfigureAppConfiguration(cb =>
                {
                    cb.AddJsonFile("Services/Location/appsettings.json", optional: false)
                    .AddEnvironmentVariables();
                }).UseStartup<LocationsTestsStartup>();

            var testServer = new TestServer(hostBuilder);

            return testServer;
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
