namespace FunctionalTests.Services.Marketing
{
    using UserLocation = Microsoft.eShopOnContainers.Services.Locations.API.Model.UserLocation;
    using LocationRequest = Microsoft.eShopOnContainers.Services.Locations.API.ViewModel.LocationRequest;
    using FunctionalTests.Services.Locations;
    using Newtonsoft.Json;
    using System;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;
    using System.Collections.Generic;
    using Microsoft.eShopOnContainers.Services.Marketing.API.Dto;
    using Microsoft.eShopOnContainers.Services.Catalog.API.ViewModel;

    public class MarketingScenarios : MarketingScenariosBase
    {
        [Fact]
        public async Task Set_new_user_location_and_get_location_campaign_by_user_id()
        {
            using (var locationsServer = new LocationsScenariosBase().CreateServer())
            using (var marketingServer = new MarketingScenariosBase().CreateServer())
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

                await Task.Delay(300);

                //Get campaing from Marketing.API given a userId
                var UserLocationCampaignResponse = await marketingServer.CreateClient()
                    .GetAsync(CampaignScenariosBase.Get.UserCampaignsByUserId(userId));

                responseBody = await UserLocationCampaignResponse.Content.ReadAsStringAsync();
                var userLocationCampaigns = JsonConvert.DeserializeObject<PaginatedItemsViewModel<CampaignDTO>>(responseBody);

                Assert.True(userLocationCampaigns.Data != null);
            }
        }
    }
}
