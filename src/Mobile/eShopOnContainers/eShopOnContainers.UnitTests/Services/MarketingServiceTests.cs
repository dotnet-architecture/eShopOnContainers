using eShopOnContainers.Core;
using eShopOnContainers.Core.Services.Marketing;
using System.Threading.Tasks;
using Xunit;

namespace eShopOnContainers.UnitTests.Services
{
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

            Assert.NotEmpty(result);
        }
    }
}