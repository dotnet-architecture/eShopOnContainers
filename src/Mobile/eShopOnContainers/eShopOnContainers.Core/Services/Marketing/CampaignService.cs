namespace eShopOnContainers.Core.Services.Marketing
{
    using System;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using Models.Marketing;
    using RequestProvider;

    public class CampaignService : ICampaignService
    {
        private readonly IRequestProvider _requestProvider;

        public CampaignService(IRequestProvider requestProvider)
        {
            _requestProvider = requestProvider;
        }

        public async Task<ObservableCollection<Campaign>> GetAllCampaignsAsync(string userId, string token)
        {
            UriBuilder builder = new UriBuilder(GlobalSetting.Instance.MarketingEndpoint);

            builder.Path = $"api/v1/campaigns/{userId}";

            string uri = builder.ToString();

            return await _requestProvider.GetAsync<ObservableCollection<Campaign>>(uri, token);
        }

        public Task<Campaign> GetCampaignByIdAsync(int campaignId, string token)
        {
            throw new NotImplementedException();
        }
    }
}