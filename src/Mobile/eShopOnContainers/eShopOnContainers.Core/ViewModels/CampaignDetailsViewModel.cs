using System;

namespace eShopOnContainers.Core.ViewModels
{
    using System.Threading.Tasks;
    using System.Windows.Input;
    using eShopOnContainers.Core.Helpers;
    using Xamarin.Forms;
    using Models.Marketing;
    using Services.Marketing;
    using Base;

    public class CampaignDetailsViewModel : ViewModelBase
    {
        private CampaignItem _campaign;
        private readonly ICampaignService _campaignService;
        private string _campaignAvailability;


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

        public string Name
        {
            get =>  _campaign.Name;
            set
            {
                _campaign.Name = value;
                RaisePropertyChanged(() => Name);
            }
        } 

        public string Description
        {
            get => _campaign.Description;
            set
            {
                _campaign.Description = value;
                RaisePropertyChanged(() => Description);
            }
        }

        public string PictureUri
        {
            get => _campaign.PictureUri;
            set
            {
                _campaign.PictureUri = value;
                RaisePropertyChanged(() => PictureUri);
            }
        }

        public string From
        {
            get => _campaign.From.ToString("MMMM dd, yyyy");
        }

        public string To
        {
            get => _campaign.To.ToString("MMMM dd, yyyy");
        }

        //public string CampaignAvailability
        //{
        //    get => $"{_campaign.From:MMMM dd, yyyy} until {_campaign.To:MMMM dd, yyyy}";
        //    set
        //    {
        //        _campaignAvailability = value;
        //        RaisePropertyChanged(() => CampaignAvailability);
        //    }
        //}


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