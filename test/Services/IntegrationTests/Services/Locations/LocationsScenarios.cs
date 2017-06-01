using Microsoft.eShopOnContainers.Services.Locations.API.ViewModel;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace IntegrationTests.Services.Locations
{
    public class LocationsScenarios
        : LocationsScenarioBase
    {
        [Fact]
        public async Task Set_new_user_location_response_ok_status_code()
        {
            using (var server = CreateServer())
            {
                var content = new StringContent(BuildLocationsRequest(), UTF8Encoding.UTF8, "application/json");

                var response = await server.CreateClient()
                    .PostAsync(Post.AddNewLocation, content);

                response.EnsureSuccessStatusCode();
            }
        }

        string BuildLocationsRequest()
        {
            var location = new LocationRequest()
            {
                Longitude = -122.333875,
                Latitude = 47.602050
            };
            return JsonConvert.SerializeObject(location);
        }
    }
}
