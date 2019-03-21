using Microsoft.eShopOnContainers.Mobile.Shopping.HttpAggregator.Config;
using Microsoft.eShopOnContainers.Mobile.Shopping.HttpAggregator.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.eShopOnContainers.Mobile.Shopping.HttpAggregator.Services
{
    public class BasketService : IBasketService
    {

        private readonly HttpClient _httpClient;
        private readonly ILogger<BasketService> _logger;
        private readonly UrlsConfig _urls;

        public BasketService(HttpClient httpClient, ILogger<BasketService> logger, IOptions<UrlsConfig> config)
        {
            _httpClient = httpClient;
            _logger = logger;
            _urls = config.Value;
        }

        public async Task<BasketData> GetByIdAsync(string id)
        {
            var data = await _httpClient.GetStringAsync(_urls.Basket + UrlsConfig.BasketOperations.GetItemById(id));

            var basket = !string.IsNullOrEmpty(data) ? JsonConvert.DeserializeObject<BasketData>(data) : null;

            return basket;
        }

        public async Task UpdateAsync(BasketData currentBasket)
        {
            var basketContent = new StringContent(JsonConvert.SerializeObject(currentBasket), System.Text.Encoding.UTF8, "application/json");

            var data = await _httpClient.PostAsync(_urls.Basket + UrlsConfig.BasketOperations.UpdateBasket(), basketContent);
        }
    }
}
