namespace eShopOnContainers.UnitTests.ViewModels
{
    using System.Threading.Tasks;
    using Xunit;
    using Core.ViewModels.Base;
    using Core.Services.Marketing;
    using Core.ViewModels;

    public class MarketingViewModelTests
    {
        public MarketingViewModelTests()
        {
            ViewModelLocator.RegisterDependencies(true);
        }

        [Fact]
        public void GetCampaignsIsNullTest()
        {
            var campaignService = new CampaignMockService();
            var campaignViewModel = new CampaignViewModel(campaignService);
            Assert.Null(campaignViewModel.Campaigns);
        }

        [Fact]
        public async Task GetCampaignsIsNotNullTest()
        {
            var campaignService = new CampaignMockService();
            var campaignViewModel = new CampaignViewModel(campaignService);

            await campaignViewModel.InitializeAsync(null);

            Assert.NotNull(campaignViewModel.Campaigns);
        }

        [Fact]
        public void GetCampaignDetailsCommandIsNotNullTest()
        {
            var campaignService = new CampaignMockService();
            var campaignViewModel = new CampaignViewModel(campaignService);
            Assert.NotNull(campaignViewModel.GetCampaignDetailsCommand);
        }

        [Fact]
        public void GetCampaignDetailsByIdIsNullTest()
        {
            var campaignService = new CampaignMockService();
            var campaignViewModel = new CampaignDetailsViewModel(campaignService);
            Assert.Null(campaignViewModel.Campaign);
        }

        [Fact]
        public async Task GetCampaignDetailsByIdIsNotNullTest()
        {
            var campaignService = new CampaignMockService();
            var campaignDetailsViewModel = new CampaignDetailsViewModel(campaignService);

            await campaignDetailsViewModel.InitializeAsync(1);

            Assert.NotNull(campaignDetailsViewModel.Campaign);
        }
    }
}