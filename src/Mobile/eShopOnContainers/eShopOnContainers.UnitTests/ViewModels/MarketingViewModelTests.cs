using eShopOnContainers.Core.Services.Marketing;
using eShopOnContainers.Core.Services.Settings;
using eShopOnContainers.Core.ViewModels;
using eShopOnContainers.Core.ViewModels.Base;
using eShopOnContainers.UnitTests.Mocks;
using System.Threading.Tasks;
using Xunit;

namespace eShopOnContainers.UnitTests.ViewModels
{
    public class MarketingViewModelTests
    {
        public MarketingViewModelTests()
        {
            ViewModelLocator.UpdateDependencies(true);
            ViewModelLocator.RegisterSingleton<ISettingsService, MockSettingsService>();
        }

        [Fact]
        public void GetCampaignsIsNullTest()
        {
            var settingsService = new MockSettingsService();
            var campaignService = new CampaignMockService();
            var campaignViewModel = new CampaignViewModel(settingsService, campaignService);
            Assert.Null(campaignViewModel.Campaigns);
        }

        [Fact]
        public async Task GetCampaignsIsNotNullTest()
        {
            var settingsService = new MockSettingsService();
            var campaignService = new CampaignMockService();
            var campaignViewModel = new CampaignViewModel(settingsService, campaignService);

            await campaignViewModel.InitializeAsync(null);

            Assert.NotNull(campaignViewModel.Campaigns);
        }

        [Fact]
        public void GetCampaignDetailsCommandIsNotNullTest()
        {
            var settingsService = new MockSettingsService();
            var campaignService = new CampaignMockService();
            var campaignViewModel = new CampaignViewModel(settingsService, campaignService);
            Assert.NotNull(campaignViewModel.GetCampaignDetailsCommand);
        }

        [Fact]
        public void GetCampaignDetailsByIdIsNullTest()
        {
            var settingsService = new MockSettingsService();
            var campaignService = new CampaignMockService();
            var campaignViewModel = new CampaignDetailsViewModel(settingsService, campaignService);
            Assert.Null(campaignViewModel.Campaign);
        }

        [Fact]
        public async Task GetCampaignDetailsByIdIsNotNullTest()
        {
            var settingsService = new MockSettingsService();
            var campaignService = new CampaignMockService();
            var campaignDetailsViewModel = new CampaignDetailsViewModel(settingsService, campaignService);

            await campaignDetailsViewModel.InitializeAsync(1);

            Assert.NotNull(campaignDetailsViewModel.Campaign);
        }
    }
}