using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System.IO;

namespace IntegrationTests.Services.Locations
{
    public class LocationsScenarioBase
    {
        public TestServer CreateServer()
        {
            var webHostBuilder = WebHost.CreateDefaultBuilder();
            webHostBuilder.UseContentRoot(Directory.GetCurrentDirectory() + "\\Services\\Locations");
            webHostBuilder.UseStartup<LocationsTestsStartup>();

            return new TestServer(webHostBuilder);
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
