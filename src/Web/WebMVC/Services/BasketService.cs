using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.eShopOnContainers.WebMVC.ViewModels;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WebMVC.Services.Utilities;

namespace Microsoft.eShopOnContainers.WebMVC.Services
{
    public class BasketService : IBasketService
    {
        private readonly IOptionsSnapshot<AppSettings> _settings;
        private IHttpClient _apiClient;
        private readonly string _remoteServiceBaseUrl;
        private IHttpContextAccessor _httpContextAccesor;

        public BasketService(IOptionsSnapshot<AppSettings> settings, IHttpContextAccessor httpContextAccesor, IHttpClient httpClient)
        {
            _settings = settings;
            _remoteServiceBaseUrl = _settings.Value.BasketUrl;
            _httpContextAccesor = httpContextAccesor;
            _apiClient = httpClient;
        }

        public async Task<Basket> GetBasket(ApplicationUser user)
        {
            var context = _httpContextAccesor.HttpContext;
            var token = await context.Authentication.GetTokenAsync("access_token");

            _apiClient.Inst.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

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
            var context = _httpContextAccesor.HttpContext;
            var token = await context.Authentication.GetTokenAsync("access_token");
           
            _apiClient.Inst.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var basketUrl = _remoteServiceBaseUrl;
            
            var response = await _apiClient.PostAsync(basketUrl, basket);

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
            var order = new Order();
            order.Total = 0;

            basket.Items.ForEach(x =>
            {
                order.OrderItems.Add(new OrderItem()
                {
                    ProductId = int.Parse(x.ProductId),
                    
                    PictureUrl = x.PictureUrl,
                    ProductName = x.ProductName,
                    Units = x.Quantity,
                    UnitPrice = x.UnitPrice
                });
                order.Total += (x.Quantity * x.UnitPrice);
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
                    Items = new List<BasketItem>()
                };
            }

            basket.Items.Add(product);
            await UpdateBasket(basket);
        }

        public async Task CleanBasket(ApplicationUser user)
        {
            var context = _httpContextAccesor.HttpContext;
            var token = await context.Authentication.GetTokenAsync("access_token");

            _apiClient.Inst.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var basketUrl = $"{_remoteServiceBaseUrl}/{user.Id.ToString()}";
            var response = await _apiClient.DeleteAsync(basketUrl);
            
            //CCE: response status code...

        }
    }
}
