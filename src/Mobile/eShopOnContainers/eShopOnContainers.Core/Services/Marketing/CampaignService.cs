using eShopOnContainers.Core.Extensions;
using eShopOnContainers.Core.Models.Marketing;
using eShopOnContainers.Core.Services.FixUri;
using eShopOnContainers.Core.Services.RequestProvider;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace eShopOnContainers.Core.Services.Marketing
{
    public class CampaignService : ICampaignService
    {
        private readonly IRequestProvider _requestProvider;
        private readonly IFixUriService _fixUriService;

        public CampaignService(IRequestProvider requestProvider, IFixUriService fixUriService)
        {
            _requestProvider = requestProvider;
            _fixUriService = fixUriService;
        }

        public async Task<ObservableCollection<CampaignItem>> GetAllCampaignsAsync(string token)
        {
            UriBuilder builder = new UriBuilder(GlobalSetting.Instance.MarketingEndpoint);
            builder.Path = "api/v1/campaigns/user";
            string uri = builder.ToString();

            CampaignRoot campaign = await _requestProvider.GetAsync<CampaignRoot>(uri, token);

            if (campaign?.Data != null)
            {
                _fixUriService.FixCampaignItemPictureUri(campaign?.Data);
                return campaign?.Data.ToObservableCollection();
            }

            return new ObservableCollection<CampaignItem>();
        }

        public async Task<CampaignItem> GetCampaignByIdAsync(int campaignId, string token)
        {
            UriBuilder builder = new UriBuilder(GlobalSetting.Instance.MarketingEndpoint);
            builder.Path = $"api/v1/campaigns/{campaignId}";
            string uri = builder.ToString();
            return await _requestProvider.GetAsync<CampaignItem>(uri, token);
        }
    }
}