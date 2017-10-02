using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using eShopOnContainers.Core.Models.Marketing;
using eShopOnContainers.Core.Services.Marketing;
using eShopOnContainers.Core.ViewModels.Base;
using eShopOnContainers.Core.Helpers;

namespace eShopOnContainers.Core.ViewModels
{
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

        public ICommand GetCampaignDetailsCommand => new Command<CampaignItem>(async (item) => await GetCampaignDetailsAsync(item));

        public override async Task InitializeAsync(object navigationData)
        {
            IsBusy = true;

            // Get campaigns by user
            Campaigns = await _campaignService.GetAllCampaignsAsync(Settings.AuthAccessToken);

            IsBusy = false;
        }

        private async Task GetCampaignDetailsAsync(CampaignItem campaign)
        {
            await NavigationService.NavigateToAsync<CampaignDetailsViewModel>(campaign.Id);
        }
    }
}