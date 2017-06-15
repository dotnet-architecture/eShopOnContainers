namespace Microsoft.eShopOnContainers.WebMVC.Services
{
    using global::WebMVC.Infrastructure;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Http;
    using Microsoft.eShopOnContainers.BuildingBlocks.Resilience.Http;
    using Microsoft.eShopOnContainers.WebMVC.Models;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
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

            _remoteServiceBaseUrl = $"{_settings.Value.MarketingUrl}/api/v1/campaigns/";
            _httpContextAccesor = httpContextAccesor ?? throw new ArgumentNullException(nameof(httpContextAccesor));
        }

        public async Task<IEnumerable<CampaignDTO>> GetCampaigns()
        {
            var userId = GetUserIdentity();
            var allCampaignItemsUri = API.Marketing.GetAllCampaigns(_remoteServiceBaseUrl, Guid.Parse(userId));

            var authorizationToken = await GetUserTokenAsync();
            var dataString = await _apiClient.GetStringAsync(allCampaignItemsUri, authorizationToken);

            var response = JsonConvert.DeserializeObject<IEnumerable<CampaignDTO>>(dataString);

            return response;
        }

        public async Task<CampaignDTO> GetCampaignById(int id)
        {
            var userId = GetUserIdentity();
            var campaignByIdItemUri = API.Marketing.GetAllCampaignById(_remoteServiceBaseUrl, id);

            var authorizationToken = await GetUserTokenAsync();
            var dataString = await _apiClient.GetStringAsync(campaignByIdItemUri, authorizationToken);

            var response = JsonConvert.DeserializeObject<CampaignDTO>(dataString);

            return response;
        }

        private string GetUserIdentity()
        {
            return _httpContextAccesor.HttpContext.User.FindFirst("sub").Value;
        }

        private async Task<string> GetUserTokenAsync()
        {
            var context = _httpContextAccesor.HttpContext;
            return await context.Authentication.GetTokenAsync("access_token");
        }
    }
}