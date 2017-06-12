namespace IntegrationTests.Services.Marketing
{
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;
    using System;
    using Newtonsoft.Json;
    using System.Net;
    using Microsoft.eShopOnContainers.Services.Marketing.API.Dto;

    public class UserLocationRoleScenarios
        : UserLocationRoleScenariosBase
    {
        [Fact]
        public async Task Get_get_all_user_location_rules_by_campaignId_and_response_ok_status_code()
        {
            var campaignId = 1;

            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                    .GetAsync(Get.UserLocationRulesByCampaignId(campaignId));

                response.EnsureSuccessStatusCode();
            }
        }

        [Fact]
        public async Task Post_add_new_user_location_rule_and_response_ok_status_code()
        {
            var campaignId = 1;

            using (var server = CreateServer())
            {
                var fakeCampaignDto = GetFakeUserLocationRuleDto();
                var content = new StringContent(JsonConvert.SerializeObject(fakeCampaignDto), Encoding.UTF8, "application/json");
                var response = await server.CreateClient()
                    .PostAsync(Post.AddNewuserLocationRule(campaignId), content);

                response.EnsureSuccessStatusCode();
            }
        }

        [Fact]
        public async Task Delete_delete_user_location_role_and_response_not_content_status_code()
        {
            var campaignId = 1;

            using (var server = CreateServer())
            {
                var fakeCampaignDto = GetFakeUserLocationRuleDto();
                var content = new StringContent(JsonConvert.SerializeObject(fakeCampaignDto), Encoding.UTF8, "application/json");

                //add user location role
                var campaignResponse = await server.CreateClient()
                    .PostAsync(Post.AddNewuserLocationRule(campaignId), content);

                if (int.TryParse(campaignResponse.Headers.Location.Segments[6], out int userLocationRuleId))
                {
                    var response = await server.CreateClient()
                    .DeleteAsync(Delete.UserLocationRoleBy(campaignId, userLocationRuleId));

                    Assert.True(response.StatusCode == HttpStatusCode.NoContent);
                }

                campaignResponse.EnsureSuccessStatusCode();
            }
        }

        private static UserLocationRuleDTO GetFakeUserLocationRuleDto()
        {
            return new UserLocationRuleDTO
            {
                LocationId = 20,
                Description = "FakeUserLocationRuleDescription"
            };
        }
    }
}
