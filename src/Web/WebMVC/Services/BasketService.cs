using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.eShopOnContainers.WebMVC.Models;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Microsoft.eShopOnContainers.WebMVC.Services
{
    public class BasketService : IBasketService
    {
        private readonly IOptions<AppSettings> _settings;
        private HttpClient _apiClient;
        private readonly string _remoteServiceBaseUrl;

        public BasketService(IOptions<AppSettings> settings)
        {
            _settings = settings;
            _remoteServiceBaseUrl = _settings.Value.BasketUrl;
        }

        public async Task<Basket> GetBasket(ApplicationUser user)
        {
            _apiClient = new HttpClient();
            var basketUrl = $"{_remoteServiceBaseUrl}/{user.Id.ToString()}";
            var dataString = await _apiClient.GetStringAsync(basketUrl);
            var response = JsonConvert.DeserializeObject<Basket>(dataString);
            if (response == null)
            {
                response = new Basket()
                {
                    BuyerId = user.Id
                };
            }

            return response;
        }

        public async Task<Basket> UpdateBasket(Basket basket)
        {
            _apiClient = new HttpClient();
            var basketUrl = _remoteServiceBaseUrl;
            StringContent content = new StringContent(JsonConvert.SerializeObject(basket), System.Text.Encoding.UTF8, "application/json");
            var response = await _apiClient.PostAsync(basketUrl, content);

            return basket;
        }

        public async Task<Basket> SetQuantities(ApplicationUser user, Dictionary<string, int> quantities)
        {
            var basket = await GetBasket(user);

            basket.Items.ForEach(x =>
            {
                var quantity = quantities.Where(y => y.Key == x.Id).FirstOrDefault();
                if (quantities.Where(y => y.Key == x.Id).Count() > 0)
                    x.Quantity = quantity.Value;
            });

            return basket;
        }

        public Order MapBasketToOrder(Basket basket)
        {
            var order = new Order()
            {
                Id = Guid.NewGuid().ToString(),
                BuyerId = basket.BuyerId
            };

            basket.Items.ForEach(x =>
            {
                order.OrderItems.Add(new OrderItem()
                {
                    ProductId = x.ProductId,
                    OrderId = order.Id,
                    PictureUrl = x.PictureUrl,
                    ProductName = x.ProductName,
                    Quantity = x.Quantity,
                    UnitPrice = x.UnitPrice
                });
            });

            return order;
        }

        public async Task AddItemToBasket(ApplicationUser user, BasketItem product)
        {
            Basket basket = await GetBasket(user);
            if (basket == null)
            {
                basket = new Basket()
                {
                    BuyerId = user.Id,
                    //Id = Guid.NewGuid().ToString(),
                    Items = new List<Models.BasketItem>()
                };
            }

            basket.Items.Add(product);
            await UpdateBasket(basket);
        }

        public async Task CleanBasket(ApplicationUser user)
        {
            _apiClient = new HttpClient();
            var basketUrl = $"{_remoteServiceBaseUrl}/{user.Id.ToString()}";
            var response = await _apiClient.DeleteAsync(basketUrl);
            
            //CCE: response status code...

        }
    }
}
