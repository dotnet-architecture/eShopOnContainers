namespace Microsoft.eShopOnContainers.WebMVC.Services
{
    using global::WebMVC.Infrastructure;
    using AspNetCore.Authentication;
    using AspNetCore.Http;
    using BuildingBlocks.Resilience.Http;
    using ViewModels;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using System;
    using System.Threading.Tasks;

    public class CampaignService : ICampaignService
    {
        private readonly IOptionsSnapshot<AppSettings> _settings;
        private readonly IHttpClient _apiClient;
        private readonly ILogger<CampaignService> _logger;
        private readonly string _remoteServiceBaseUrl;
        private readonly IHttpContextAccessor _httpContextAccesor;

        public CampaignService(IOptionsSnapshot<AppSettings> settings, IHttpClient httpClient,
            ILogger<CampaignService> logger, IHttpContextAccessor httpContextAccesor)
        {
            _settings = settings;
            _apiClient = httpClient;
            _logger = logger;

            _remoteServiceBaseUrl = $"{_settings.Value.MarketingUrl}/api/v1/m/campaigns/";
            _httpContextAccesor = httpContextAccesor ?? throw new ArgumentNullException(nameof(httpContextAccesor));
        }

        public async Task<Campaign> GetCampaigns(int pageSize, int pageIndex)
        {
            var allCampaignItemsUri = API.Marketing.GetAllCampaigns(_remoteServiceBaseUrl, 
                pageSize, pageIndex);

            var authorizationToken = await GetUserTokenAsync();
            var dataString = await _apiClient.GetStringAsync(allCampaignItemsUri, authorizationToken);

            var response = JsonConvert.DeserializeObject<Campaign>(dataString);

            return response;
        }

        public async Task<CampaignItem> GetCampaignById(int id)
        {
            var campaignByIdItemUri = API.Marketing.GetAllCampaignById(_remoteServiceBaseUrl, id);

            var authorizationToken = await GetUserTokenAsync();
            var dataString = await _apiClient.GetStringAsync(campaignByIdItemUri, authorizationToken);

            var response = JsonConvert.DeserializeObject<CampaignItem>(dataString);

            return response;
        }

        private string GetUserIdentity()
        {
            return _httpContextAccesor.HttpContext.User.FindFirst("sub").Value;
        }

        private async Task<string> GetUserTokenAsync()
        {
            var context = _httpContextAccesor.HttpContext;
            return await context.GetTokenAsync("access_token");
        }
    }
}