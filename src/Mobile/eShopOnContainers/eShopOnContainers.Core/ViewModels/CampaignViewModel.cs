using System.Threading.Tasks;
using System.Windows.Input;
using eShopOnContainers.Core.Helpers;
using eShopOnContainers.Core.Models.Catalog;
using eShopOnContainers.Core.Models.User;
using Xamarin.Forms;

namespace eShopOnContainers.Core.ViewModels
{
    using System.Collections.ObjectModel;
    using Models.Marketing;
    using Services.Marketing;
    using Base;

    public class CampaignViewModel : ViewModelBase
    {
        private ObservableCollection<Campaign> _campaigns;
        private readonly ICampaignService _campaignService;

        public CampaignViewModel(ICampaignService campaignService)
        {
            _campaignService = campaignService;
        }

        public ObservableCollection<Campaign> Campaigns
        {
            get => _campaigns;
            set
            {
                _campaigns = value;
                RaisePropertyChanged(() => Campaigns);
            }
        }

        public ICommand GetCampaignDetailsCommand => new Command<Campaign>(GetCampaignDetails);

        public override async Task InitializeAsync(object navigationData)
        {
            IsBusy = true;

            // Get campaigns by user
            Campaigns = await _campaignService.GetAllCampaignsAsync(Settings.UserId, Settings.AuthAccessToken);

            IsBusy = false;
        }

        private void GetCampaignDetails(Campaign campaign)
        {
            
        }
    }
}