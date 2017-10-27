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
                    Latitude = 47.60461
                };
                var content = new StringContent(JsonConvert.SerializeObject(location),
                    Encoding.UTF8, "application/json");

                // GIVEN a new location of user is created 
                await locationsServer.CreateClient()
                    .PostAsync(LocationsScenariosBase.Post.AddNewLocation, content);

                await Task.Delay(300);

                //Get campaing from Marketing.API
                var campaignsResponse = await marketingServer.CreateClient()
                    .GetAsync(CampaignScenariosBase.Get.Campaigns);

                var responseBody = await campaignsResponse.Content.ReadAsStringAsync();
                var campaigns = JsonConvert.DeserializeObject<List<CampaignDTO>>(responseBody);

                Assert.True(campaigns.Count > 0);
            }
        }
    }
}
