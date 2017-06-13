namespace FunctionalTests.Services.Marketing
{
    using UserLocationDTO = Microsoft.eShopOnContainers.Services.Marketing.API.Dto.UserLocationDTO;
    using UserLocation = Microsoft.eShopOnContainers.Services.Locations.API.Model.UserLocation;
    using LocationRequest = Microsoft.eShopOnContainers.Services.Locations.API.ViewModel.LocationRequest;
    using FunctionalTests.Extensions;
    using FunctionalTests.Services.Basket;
    using FunctionalTests.Services.Locations;
    using Microsoft.eShopOnContainers.Services.Basket.API.Model;
    using Microsoft.eShopOnContainers.WebMVC.ViewModels;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using WebMVC.Models;
    using Xunit;

    public class MarketingScenarios : MarketingScenariosBase
    {
        [Fact]
        public async Task Set_new_user_location_and_get_location_campaign_by_user_id()
        {
            using (var locationsServer = new LocationsScenariosBase().CreateServer())
            using (var marketingServer = CreateServer())
            {
                var location = new LocationRequest
                {
                    Longitude = -122.315752,
                    Latitude = 47.604610
                };
                var content = new StringContent(JsonConvert.SerializeObject(location),
                    Encoding.UTF8, "application/json");

                var userId = new Guid("4611ce3f-380d-4db5-8d76-87a8689058ed");
                

                // GIVEN a new location of user is created 
                var response = await locationsServer.CreateClient()
                    .PostAsync(LocationsScenariosBase.Post.AddNewLocation, content);

                //Get location user from Location.API
                var userLocationResponse = await locationsServer.CreateClient()
                    .GetAsync(LocationsScenariosBase.Get.UserLocationBy(userId));

                var responseBody = await userLocationResponse.Content.ReadAsStringAsync();
                var userLocation = JsonConvert.DeserializeObject<UserLocation>(responseBody);

                await Task.Delay(5000);

                //Get campaing from Marketing.API given a userId
                var UserLocationCampaignResponse = await locationsServer.CreateClient()
                    .GetAsync(Get.UserCampaignByUserId(userId));

                responseBody = await UserLocationCampaignResponse.Content.ReadAsStringAsync();
                var userLocationCampaign = JsonConvert.DeserializeObject<UserLocationDTO>(responseBody);

                // Assert
                Assert.Equal(userLocation.LocationId, userLocationCampaign.LocationId);
            }
        }
    }
}
