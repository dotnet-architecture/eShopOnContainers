namespace eShopOnContainers.UnitTests.Services
{
    using System.Threading.Tasks;
    using Core;
    using Core.Helpers;
    using Core.Services.Marketing;
    using Xunit;

    public class MarketingServiceTests
    {
        [Fact]
        public async Task GetFakeCampaigTest()
        {
            var campaignMockService = new CampaignMockService();
            var order = await campaignMockService.GetCampaignByIdAsync(1, GlobalSetting.Instance.AuthToken);

            Assert.NotNull(order);
        }

        [Fact]
        public async Task GetFakeCampaignsTest()
        {
            var campaignMockService = new CampaignMockService();
            var result = await campaignMockService.GetAllCampaignsAsync(GlobalSetting.Instance.AuthToken);

            Assert.NotEqual(0, result.Count);
        }
    }
}