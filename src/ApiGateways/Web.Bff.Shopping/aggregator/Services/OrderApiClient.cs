using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.eShopOnContainers.BuildingBlocks.Resilience.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Microsoft.eShopOnContainers.Web.Shopping.HttpAggregator.Config;
using Microsoft.eShopOnContainers.Web.Shopping.HttpAggregator.Models;

namespace Microsoft.eShopOnContainers.Web.Shopping.HttpAggregator.Services
{
    public class OrderApiClient : IOrderApiClient
    {

        private readonly IHttpClient _apiClient;
        private readonly ILogger<OrderApiClient> _logger;
        private readonly UrlsConfig _urls;

        public OrderApiClient(IHttpClient httpClient, ILogger<OrderApiClient> logger, IOptionsSnapshot<UrlsConfig> config)
        {
            _apiClient = httpClient;
            _logger = logger;
            _urls = config.Value;
        }

        public async Task<OrderData> GetOrderDraftFromBasket(BasketData basket)
        {
            var url = _urls.Orders + UrlsConfig.OrdersOperations.GetOrderDraft();
            var response = await _apiClient.PostAsync<BasketData>(url, basket);
            response.EnsureSuccessStatusCode();
            var jsonResponse = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<OrderData>(jsonResponse);
        }
    }
}
