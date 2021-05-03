using Microsoft.eShopOnContainers.Mobile.Shopping.HttpAggregator.Config;
using Microsoft.eShopOnContainers.Mobile.Shopping.HttpAggregator.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;

namespace Microsoft.eShopOnContainers.Mobile.Shopping.HttpAggregator.Services
{
    public class OrderApiClient : IOrderApiClient
    {
        private readonly HttpClient _apiClient;
        private readonly ILogger<OrderApiClient> _logger;
        private readonly UrlsConfig _urls;

        public OrderApiClient(HttpClient httpClient, ILogger<OrderApiClient> logger, IOptions<UrlsConfig> config)
        {
            _apiClient = httpClient;
            _logger = logger;
            _urls = config.Value;
        }

        public async Task<OrderData> GetOrderDraftFromBasketAsync(BasketData basket)
        {
            var uri = _urls.Orders + UrlsConfig.OrdersOperations.GetOrderDraft();
            var content = new StringContent(JsonSerializer.Serialize(basket), System.Text.Encoding.UTF8, "application/json");
            var response = await _apiClient.PostAsync(uri, content);

            response.EnsureSuccessStatusCode();

            var ordersDraftResponse = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<OrderData>(ordersDraftResponse, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
    }
}
