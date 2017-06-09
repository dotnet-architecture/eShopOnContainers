using Microsoft.eShopOnContainers.Services.Locations.API.Model;
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
        public async Task Set_new_user_seattle_location_response_ok_status_code()
        {
            using (var server = CreateServer())
            {
                var userId = 1234;
                var content = new StringContent(BuildLocationsRequest(-122.315752, 47.604610), UTF8Encoding.UTF8, "application/json");

                var response = await server.CreateClient()
                    .PostAsync(Post.AddNewLocation, content);

                response.EnsureSuccessStatusCode();

                var userLocationResponse = await server.CreateClient()
                    .GetAsync(Get.LocationBy(userId));

                var responseBody = await userLocationResponse.Content.ReadAsStringAsync();
                var userLocation = JsonConvert.DeserializeObject<UserLocation>(responseBody);

                response.EnsureSuccessStatusCode();


            }
        }

        string BuildLocationsRequest(double lon, double lat)
        {
            var location = new LocationRequest()
            {
                Longitude = lon,
                Latitude = lat
            }; 
            return JsonConvert.SerializeObject(location);
        }
    }
}
