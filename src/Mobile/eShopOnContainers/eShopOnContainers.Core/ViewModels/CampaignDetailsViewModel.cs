namespace eShopOnContainers.Core.ViewModels
{
    using System.Threading.Tasks;
    using Helpers;
    using Models.Marketing;
    using Services.Marketing;
    using Base;
    using System.Windows.Input;
    using Xamarin.Forms;

    public class CampaignDetailsViewModel : ViewModelBase
    {
        private CampaignItem _campaign;
        private bool _isDetailsSite;
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

        public bool IsDetailsSite
        {
            get => _isDetailsSite;
            set
            {
                _isDetailsSite = value;
                RaisePropertyChanged(() => IsDetailsSite);
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

        public ICommand EnableDetailsSiteCommand => new Command(EnableDetailsSite);

        private void EnableDetailsSite()
        {
            IsDetailsSite = true;
        }
    }
}