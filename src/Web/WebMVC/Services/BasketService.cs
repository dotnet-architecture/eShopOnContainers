using Microsoft.eShopOnContainers.WebMVC.ViewModels;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WebMVC.Infrastructure;
using WebMVC.Models;

namespace Microsoft.eShopOnContainers.WebMVC.Services
{
    public class BasketService : IBasketService
    {
        private readonly IOptionsSnapshot<AppSettings> _settings;
        private readonly HttpClient _apiClient;
        private readonly string _basketByPassUrl;
        private readonly string _purchaseUrl;

        private readonly string _bffUrl;

        public BasketService(HttpClient httpClient,IOptionsSnapshot<AppSettings> settings)
        {
            _apiClient = httpClient;
            _settings = settings;

            _basketByPassUrl = $"{_settings.Value.PurchaseUrl}/api/v1/b/basket";
            _purchaseUrl = $"{_settings.Value.PurchaseUrl}/api/v1";
        }

        public async Task<Basket> GetBasket(ApplicationUser user)
        {
            var getBasketUri = API.Basket.GetBasket(_basketByPassUrl, user.Id);

            var dataString = await _apiClient.GetStringAsync(getBasketUri);

            return string.IsNullOrEmpty(dataString) ? 
                new Basket() {  BuyerId = user.Id} :
                JsonConvert.DeserializeObject<Basket>(dataString);
        }

        public async Task<Basket> UpdateBasket(Basket basket)
        {
            var updateBasketUri = API.Basket.UpdateBasket(_basketByPassUrl);
            var content = new StringContent(JsonConvert.SerializeObject(basket), System.Text.Encoding.UTF8, "application/json");

            var response = await _apiClient.PostAsync(updateBasketUri, content);

            response.EnsureSuccessStatusCode();

            return basket;
        }

        public async Task Checkout(BasketDTO basket)
        {
            var updateBasketUri = API.Basket.CheckoutBasket(_basketByPassUrl);
            var content = new StringContent(JsonConvert.SerializeObject(basket), System.Text.Encoding.UTF8, "application/json");

            var response = await _apiClient.PostAsync(updateBasketUri, content);

            response.EnsureSuccessStatusCode();
        }

        public async Task<Basket> SetQuantities(ApplicationUser user, Dictionary<string, int> quantities)
        {

            var updateBasketUri = API.Purchase.UpdateBasketItem(_purchaseUrl);
            var basketUpdate = new
            {
                BasketId = user.Id,
                Updates = quantities.Select(kvp => new
                {
                    BasketItemId = kvp.Key,
                    NewQty = kvp.Value
                }).ToArray()
            };

            var content = new StringContent(JsonConvert.SerializeObject(basketUpdate), System.Text.Encoding.UTF8, "application/json");

            var response = await _apiClient.PutAsync(updateBasketUri,content);

            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<Basket>(jsonResponse);
        }

        public async Task<Order> GetOrderDraft(string basketId)
        {
            var draftOrderUri = API.Purchase.GetOrderDraft(_purchaseUrl, basketId);
            var response = await _apiClient.GetStringAsync(draftOrderUri);

            return JsonConvert.DeserializeObject<Order>(response);
        }



        public async Task AddItemToBasket(ApplicationUser user, int productId)
        {
            var updateBasketUri = API.Purchase.AddItemToBasket(_purchaseUrl);

            var newItem = new
            {
                CatalogItemId = productId,
                BasketId = user.Id,
                Quantity = 1
            };

            var content = new StringContent(JsonConvert.SerializeObject(newItem), System.Text.Encoding.UTF8, "application/json");

            var response = await _apiClient.PostAsync(updateBasketUri, content);
        }
    }
}
