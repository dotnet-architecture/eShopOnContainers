using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.eShopOnContainers.BuildingBlocks.Resilience.Http;
using Microsoft.eShopOnContainers.WebMVC.ViewModels;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Infrastructure;
using WebMVC.Models;

namespace Microsoft.eShopOnContainers.WebMVC.Services
{
    public class BasketService : IBasketService
    {
        private readonly IOptionsSnapshot<AppSettings> _settings;
        private readonly IHttpClient _apiClient;
        private readonly string _basketByPassUrl;
        private readonly string _purchaseUrl;
        private readonly IHttpContextAccessor _httpContextAccesor;

        private readonly string _bffUrl;

        public BasketService(IOptionsSnapshot<AppSettings> settings,
            IHttpContextAccessor httpContextAccesor, IHttpClient httpClient)
        {
            _settings = settings;
            _basketByPassUrl = $"{_settings.Value.PurchaseUrl}/api/v1/b/basket";
            _purchaseUrl = $"{_settings.Value.PurchaseUrl}/api/v1";
            _httpContextAccesor = httpContextAccesor;
            _apiClient = httpClient;
        }

        public async Task<Basket> GetBasket(ApplicationUser user)
        {
            var token = await GetUserTokenAsync();
            var getBasketUri = API.Basket.GetBasket(_basketByPassUrl, user.Id);

            var dataString = await _apiClient.GetStringAsync(getBasketUri, token);

            return string.IsNullOrEmpty(dataString) ? 
                new Basket() {  BuyerId = user.Id} :
                JsonConvert.DeserializeObject<Basket>(dataString);
        }

        public async Task<Basket> UpdateBasket(Basket basket)
        {
            var token = await GetUserTokenAsync();
            var updateBasketUri = API.Basket.UpdateBasket(_basketByPassUrl);

            var response = await _apiClient.PostAsync(updateBasketUri, basket, token);

            response.EnsureSuccessStatusCode();

            return basket;
        }

        public async Task Checkout(BasketDTO basket)
        {
            var token = await GetUserTokenAsync();
            var updateBasketUri = API.Basket.CheckoutBasket(_basketByPassUrl);

            var response = await _apiClient.PostAsync(updateBasketUri, basket, token);

            response.EnsureSuccessStatusCode();
        }

        public async Task<Basket> SetQuantities(ApplicationUser user, Dictionary<string, int> quantities)
        {

            var token = await GetUserTokenAsync();
            var updateBasketUri = API.Purchase.UpdateBasketItem(_purchaseUrl);
            var userId = user.Id;

            var response = await _apiClient.PutAsync(updateBasketUri, new
            {
                BasketId = userId,
                Updates = quantities.Select(kvp => new
                {
                    BasketItemId = kvp.Key,
                    NewQty = kvp.Value
                }).ToArray()
            }, token);

            response.EnsureSuccessStatusCode();
            var jsonResponse = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Basket>(jsonResponse);
        }

        public async Task<Order> GetOrderDraft(string basketId)
        {
            var token = await GetUserTokenAsync();
            var draftOrderUri = API.Purchase.GetOrderDraft(_purchaseUrl, basketId);
            var json = await _apiClient.GetStringAsync(draftOrderUri, token);
            return JsonConvert.DeserializeObject<Order>(json);
        }



        public async Task AddItemToBasket(ApplicationUser user, int productId)
        {
            var token = await GetUserTokenAsync();
            var updateBasketUri = API.Purchase.AddItemToBasket(_purchaseUrl);
            var userId = user.Id;

            var response = await _apiClient.PostAsync(updateBasketUri, new
            {
                CatalogItemId = productId,
                BasketId = userId,
                Quantity = 1
            }, token);

        }

        async Task<string> GetUserTokenAsync()
        {
            var context = _httpContextAccesor.HttpContext;
            return await context.GetTokenAsync("access_token");
        }
    }
}
