using Microsoft.eShopOnContainers.Services.Locations.API.Model;
using Microsoft.eShopOnContainers.Services.Locations.API.ViewModel;
using Location = Microsoft.eShopOnContainers.Services.Locations.API.Model.Locations;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using System;

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
                var userId = "4611ce3f-380d-4db5-8d76-87a8689058ed";
                var content = new StringContent(BuildLocationsRequest(-122.315752, 47.604610), UTF8Encoding.UTF8, "application/json");

                // Expected result
                var expectedLocation = "SEAT";

                // Act
                var response = await server.CreateClient()
                    .PostAsync(Post.AddNewLocation, content);

                var userLocationResponse = await server.CreateClient()
                    .GetAsync(Get.UserLocationBy(userId));

                var responseBody = await userLocationResponse.Content.ReadAsStringAsync();
                var userLocation = JsonConvert.DeserializeObject<UserLocation>(responseBody);

                var locationResponse = await server.CreateClient()
                    .GetAsync(Get.LocationBy(userLocation.LocationId));

                responseBody = await locationResponse.Content.ReadAsStringAsync();
                var location = JsonConvert.DeserializeObject<Location>(responseBody);

                // Assert
                Assert.Equal(expectedLocation, location.Code);
            }
        }

        [Fact]
        public async Task Set_new_user_readmond_location_response_ok_status_code()
        {
            using (var server = CreateServer())
            {
                var userId = "4611ce3f-380d-4db5-8d76-87a8689058ed";
                var content = new StringContent(BuildLocationsRequest(-122.119998, 47.690876), UTF8Encoding.UTF8, "application/json");

                // Expected result
                var expectedLocation = "REDM";

                // Act
                var response = await server.CreateClient()
                    .PostAsync(Post.AddNewLocation, content);

                var userLocationResponse = await server.CreateClient()
                    .GetAsync(Get.UserLocationBy(userId));

                var responseBody = await userLocationResponse.Content.ReadAsStringAsync();
                var userLocation = JsonConvert.DeserializeObject<UserLocation>(responseBody);

                var locationResponse = await server.CreateClient()
                    .GetAsync(Get.LocationBy(userLocation.LocationId));

                responseBody = await locationResponse.Content.ReadAsStringAsync();
                var location = JsonConvert.DeserializeObject<Location>(responseBody);

                // Assert
                Assert.Equal(expectedLocation, location.Code);
            }
        }

        [Fact]
        public async Task Set_new_user_Washington_location_response_ok_status_code()
        {
            using (var server = CreateServer())
            {
                var userId = "4611ce3f-380d-4db5-8d76-87a8689058ed";
                var content = new StringContent(BuildLocationsRequest(-121.040360, 48.091631), UTF8Encoding.UTF8, "application/json");

                // Expected result
                var expectedLocation = "WHT";

                // Act
                var response = await server.CreateClient()
                    .PostAsync(Post.AddNewLocation, content);

                var userLocationResponse = await server.CreateClient()
                    .GetAsync(Get.UserLocationBy(userId));

                var responseBody = await userLocationResponse.Content.ReadAsStringAsync();
                var userLocation = JsonConvert.DeserializeObject<UserLocation>(responseBody);

                var locationResponse = await server.CreateClient()
                    .GetAsync(Get.LocationBy(userLocation.LocationId));

                responseBody = await locationResponse.Content.ReadAsStringAsync();
                var location = JsonConvert.DeserializeObject<Location>(responseBody);

                // Assert
                Assert.Equal(expectedLocation, location.Code);
            }
        }

        [Fact]
        public async Task Get_all_locations_response_ok_status_code()
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                    .GetAsync(Get.Locations);                

                var responseBody = await response.Content.ReadAsStringAsync();
                var locations = JsonConvert.DeserializeObject<List<Location>>(responseBody);

                // Assert
                Assert.NotEmpty(locations);
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
