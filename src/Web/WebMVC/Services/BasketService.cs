using Microsoft.eShopOnContainers.WebMVC.ViewModels;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WebMVC.Infrastructure;
using WebMVC.Services.ModelDTOs;

namespace Microsoft.eShopOnContainers.WebMVC.Services
{
    public class BasketService : IBasketService
    {
        private readonly IOptions<AppSettings> _settings;
        private readonly HttpClient _apiClient;
        private readonly ILogger<BasketService> _logger;
        private readonly string _basketByPassUrl;
        private readonly string _purchaseUrl;

        public BasketService(HttpClient httpClient, IOptions<AppSettings> settings, ILogger<BasketService> logger)
        {
            _apiClient = httpClient;
            _settings = settings;
            _logger =logger;

            _basketByPassUrl = $"{_settings.Value.PurchaseUrl}/b/api/v1/basket";
            _purchaseUrl = $"{_settings.Value.PurchaseUrl}/api/v1";
        }

        public async Task<Basket> GetBasket(ApplicationUser user)
        {
            var uri = API.Basket.GetBasket(_basketByPassUrl, user.Id);
            _logger.LogDebug("[GetBasket] -> Calling {Uri} to get the basket", uri);
            var response = await _apiClient.GetAsync(uri);  
            _logger.LogDebug("[GetBasket] -> response code {StatusCode}", response.StatusCode);
            var responseString = await response.Content.ReadAsStringAsync();
            return string.IsNullOrEmpty(responseString) ?
                new Basket() { BuyerId = user.Id } :
                JsonConvert.DeserializeObject<Basket>(responseString);
        }

        public async Task<Basket> UpdateBasket(Basket basket)
        {
            var uri = API.Basket.UpdateBasket(_basketByPassUrl);

            var basketContent = new StringContent(JsonConvert.SerializeObject(basket), System.Text.Encoding.UTF8, "application/json");

            var response = await _apiClient.PostAsync(uri, basketContent);

            response.EnsureSuccessStatusCode();

            return basket;
        }

        public async Task Checkout(BasketDTO basket)
        {
            var uri = API.Basket.CheckoutBasket(_basketByPassUrl);
            var basketContent = new StringContent(JsonConvert.SerializeObject(basket), System.Text.Encoding.UTF8, "application/json");

            _logger.LogInformation("Uri chechout {uri}", uri);

            var response = await _apiClient.PostAsync(uri, basketContent);

            response.EnsureSuccessStatusCode();
        }

        public async Task<Basket> SetQuantities(ApplicationUser user, Dictionary<string, int> quantities)
        {
            var uri = API.Purchase.UpdateBasketItem(_purchaseUrl);

            var basketUpdate = new
            {
                BasketId = user.Id,
                Updates = quantities.Select(kvp => new
                {
                    BasketItemId = kvp.Key,
                    NewQty = kvp.Value
                }).ToArray()
            };

            var basketContent = new StringContent(JsonConvert.SerializeObject(basketUpdate), System.Text.Encoding.UTF8, "application/json");

            var response = await _apiClient.PutAsync(uri, basketContent);

            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<Basket>(jsonResponse);
        }

        public async Task<Order> GetOrderDraft(string basketId)
        {
            var uri = API.Purchase.GetOrderDraft(_purchaseUrl, basketId);

            var responseString = await _apiClient.GetStringAsync(uri);

            var response =  JsonConvert.DeserializeObject<Order>(responseString);

            return response;
        }

        public async Task AddItemToBasket(ApplicationUser user, int productId)
        {
            var uri = API.Purchase.AddItemToBasket(_purchaseUrl);

            var newItem = new
            {
                CatalogItemId = productId,
                BasketId = user.Id,
                Quantity = 1
            };

            var basketContent = new StringContent(JsonConvert.SerializeObject(newItem), System.Text.Encoding.UTF8, "application/json");

            var response = await _apiClient.PostAsync(uri, basketContent);
        }
    }
}
