using Microsoft.eShopOnContainers.Web.Shopping.HttpAggregator.Config;
using Microsoft.eShopOnContainers.Web.Shopping.HttpAggregator.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.eShopOnContainers.Web.Shopping.HttpAggregator.Services
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
            var url = _urls.Orders + UrlsConfig.OrdersOperations.GetOrderDraft();
            var content = new StringContent(JsonConvert.SerializeObject(basket), System.Text.Encoding.UTF8, "application/json");
            var response = await _apiClient.PostAsync(url, content);

            response.EnsureSuccessStatusCode();

            var ordersDraftResponse = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<OrderData>(ordersDraftResponse);
        }
    }
}
