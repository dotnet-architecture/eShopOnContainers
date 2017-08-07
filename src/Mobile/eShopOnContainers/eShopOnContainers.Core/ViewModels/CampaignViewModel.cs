namespace eShopOnContainers.Core.ViewModels
{
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Xamarin.Forms;
    using System.Collections.ObjectModel;
    using Models.Marketing;
    using Services.Marketing;
    using Base;
    using Helpers;

    public class CampaignViewModel : ViewModelBase
    {
        private ObservableCollection<CampaignItem> _campaigns;
        private readonly ICampaignService _campaignService;

        public CampaignViewModel(ICampaignService campaignService)
        {
            _campaignService = campaignService;
        }

        public ObservableCollection<CampaignItem> Campaigns
        {
            get => _campaigns;
            set
            {
                _campaigns = value;
                RaisePropertyChanged(() => Campaigns);
            }
        }

        public ICommand GetCampaignDetailsCommand => new Command<CampaignItem>(async (item) => await GetCampaignDetails(item));

        public override async Task InitializeAsync(object navigationData)
        {
            IsBusy = true;

            // Get campaigns by user
            Campaigns = await _campaignService.GetAllCampaignsAsync(Settings.AuthAccessToken);

            IsBusy = false;
        }

        private async Task GetCampaignDetails(CampaignItem campaign)
        {
            await NavigationService.NavigateToAsync<CampaignDetailsViewModel>(campaign.Id);
        }
    }
}