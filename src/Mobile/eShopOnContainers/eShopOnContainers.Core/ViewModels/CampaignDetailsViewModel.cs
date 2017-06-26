namespace eShopOnContainers.Core.ViewModels
{
    using System.Threading.Tasks;
    using Helpers;
    using Models.Marketing;
    using Services.Marketing;
    using Base;

    public class CampaignDetailsViewModel : ViewModelBase
    {
        private CampaignItem _campaign;
        private readonly ICampaignService _campaignService;

        public CampaignDetailsViewModel(ICampaignService campaignService)
        {
            _campaignService = campaignService;
        }

        public CampaignItem Campaign
        {
            get => _campaign;
            set
            {
                _campaign = value;
                RaisePropertyChanged(() => Campaign);
            }
        }

        public override async Task InitializeAsync(object navigationData)
        {
            if (navigationData is int)
            {
                IsBusy = true;

                // Get campaign by id
                Campaign = await _campaignService.GetCampaignByIdAsync((int) navigationData, Settings.AuthAccessToken);

                IsBusy = false;  
            }
        }
    }
}