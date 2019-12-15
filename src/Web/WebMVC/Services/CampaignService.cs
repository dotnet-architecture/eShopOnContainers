namespace Microsoft.eShopOnContainers.WebMVC.Services
{
    using global::WebMVC.Infrastructure;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using System.Net.Http;
    using System.Threading.Tasks;
    using ViewModels;

    public class CampaignService : ICampaignService
    {
        private readonly IOptions<AppSettings> _settings;
        private readonly HttpClient _httpClient;
        private readonly ILogger<CampaignService> _logger;
        private readonly string _remoteServiceBaseUrl;

        public CampaignService(IOptions<AppSettings> settings, HttpClient httpClient, ILogger<CampaignService> logger)
        {
            _settings = settings;
            _httpClient = httpClient;
            _logger = logger;

            _remoteServiceBaseUrl = $"{_settings.Value.MarketingUrl}/m/api/v1/campaigns/";
        }

        public async Task<Campaign> GetCampaigns(int pageSize, int pageIndex)
        {
            var uri = API.Marketing.GetAllCampaigns(_remoteServiceBaseUrl, pageSize, pageIndex);

            var responseString = await _httpClient.GetStringAsync(uri);

            var response = JsonConvert.DeserializeObject<Campaign>(responseString);

            return response;
        }

        public async Task<CampaignItem> GetCampaignById(int id)
        {
            var uri = API.Marketing.GetAllCampaignById(_remoteServiceBaseUrl, id);

            var responseString = await _httpClient.GetStringAsync(uri);

            var response = JsonConvert.DeserializeObject<CampaignItem>(responseString);

            return response;
        }
    }
}