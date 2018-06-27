using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Reflection;

namespace Locations.FunctionalTests
{
    public class LocationsScenarioBase
    {
        public TestServer CreateServer()
        {
            var path = Assembly.GetAssembly(typeof(LocationsScenarioBase))
             .Location;

            var hostBuilder = new WebHostBuilder()
                .UseContentRoot(Path.GetDirectoryName(path))
                .ConfigureAppConfiguration(cb =>
                {
                    cb.AddJsonFile("appsettings.json", optional: false)
                    .AddEnvironmentVariables();
                }).UseStartup<LocationsTestsStartup>();

            return new TestServer(hostBuilder);
        }

        public static class Get
        {
            public static string Locations = "api/v1/locations";

            public static string LocationBy(int id)
            {
                return $"api/v1/locations/{id}";
            }

            public static string UserLocationBy(string id)
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
