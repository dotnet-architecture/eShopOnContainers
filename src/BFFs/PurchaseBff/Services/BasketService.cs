using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.eShopOnContainers.BuildingBlocks.Resilience.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PurchaseBff.Config;
using PurchaseBff.Models;

namespace PurchaseBff.Services
{
    public class BasketService : IBasketService
    {

        private readonly IHttpClient _apiClient;
        private readonly ILogger<BasketService> _logger;
        private readonly UrlsConfig _urls;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BasketService(IHttpClient httpClient, IHttpContextAccessor httpContextAccessor, ILogger<BasketService> logger, IOptionsSnapshot<UrlsConfig> config)
        {
            _apiClient = httpClient;
            _logger = logger;
            _urls = config.Value;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<BasketData> GetById(string id)
        {
            var token = await GetUserTokenAsync();
            var data = await _apiClient.GetStringAsync(_urls.Basket +  UrlsConfig.BasketOperations.GetItemById(id), token);
            var basket = JsonConvert.DeserializeObject<BasketData>(data);
            return basket;
        }

        public async Task Update(BasketData currentBasket)
        {
            var token = await GetUserTokenAsync();
            var data = await _apiClient.PostAsync<BasketData>(_urls.Basket + UrlsConfig.BasketOperations.UpdateBasket(), currentBasket, token);
            int i = 0;
        }

        async Task<string> GetUserTokenAsync()
        {
            var context = _httpContextAccessor.HttpContext;
            return await context.GetTokenAsync("access_token");
        }
    }
}
