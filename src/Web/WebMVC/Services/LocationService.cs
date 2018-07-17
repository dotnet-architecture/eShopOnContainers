using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.eShopOnContainers.BuildingBlocks.Resilience.Http;
using Microsoft.eShopOnContainers.WebMVC;
using Microsoft.eShopOnContainers.WebMVC.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using WebMVC.Infrastructure;
using WebMVC.Models;

namespace WebMVC.Services
{
    public class LocationService : ILocationService
    {
        private readonly IOptionsSnapshot<AppSettings> _settings;
        private readonly IHttpClient _apiClient;
        private readonly ILogger<CampaignService> _logger;
        private readonly string _remoteServiceBaseUrl;
        private readonly IHttpContextAccessor _httpContextAccesor;

        public LocationService(IOptionsSnapshot<AppSettings> settings, IHttpClient httpClient,
            ILogger<CampaignService> logger, IHttpContextAccessor httpContextAccesor)
        {
            _settings = settings;
            _apiClient = httpClient;
            _logger = logger;

            _remoteServiceBaseUrl = $"{_settings.Value.MarketingUrl}/api/v1/l/locations/";
            _httpContextAccesor = httpContextAccesor ?? throw new ArgumentNullException(nameof(httpContextAccesor));
        }

        public async Task CreateOrUpdateUserLocation(LocationDTO location)
        {
            var createOrUpdateUserLocationUri = API.Locations.CreateOrUpdateUserLocation(_remoteServiceBaseUrl);

            var authorizationToken = await GetUserTokenAsync();
            var response = await _apiClient.PostAsync(createOrUpdateUserLocationUri, location, authorizationToken);
            response.EnsureSuccessStatusCode();
        }      

        private async Task<string> GetUserTokenAsync()
        {
            var context = _httpContextAccesor.HttpContext;
            return await context.GetTokenAsync("access_token");
        }
    }
}
