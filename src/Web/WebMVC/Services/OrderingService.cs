using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.eShopOnContainers.WebMVC.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Net.Http;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authentication;

namespace Microsoft.eShopOnContainers.WebMVC.Services
{
    public class OrderingService : IOrderingService
    {
        private HttpClient _apiClient;
        private readonly string _remoteServiceBaseUrl;
        private readonly IOptions<AppSettings> _settings;
        private readonly IHttpContextAccessor _httpContextAccesor;

        public OrderingService(IOptions<AppSettings> settings, IHttpContextAccessor httpContextAccesor)
        {
            _remoteServiceBaseUrl = $"{settings.Value.OrderingUrl}/api/v1/orders";
            _settings = settings;
            _httpContextAccesor = httpContextAccesor;

            #region fake items
            //_orders = new List<Order>()
            //{
            //    new Order()
            //    {
            //        Id = Guid.NewGuid().ToString(),
            //        BuyerId = new Guid("ebcbcb4c-b032-4baa-834b-7fd66d37bc95").ToString(),
            //        OrderDate = DateTime.Now,
            //        State = OrderState.InProcess,
            //        OrderItems = new List<OrderItem>()
            //        {
            //            new OrderItem() { UnitPrice = 12.40m, PictureUrl = "https://fakeimg.pl/370x240/EEEEEE/000/?text=RoslynRedT-Shirt", Quantity = 1, ProductName="Roslyn Red T-Shirt" }
            //        }
            //    }, 
            //    new Order()
            //    {
            //        Id = Guid.NewGuid().ToString(),
            //        BuyerId = new Guid("ebcbcb4c-b032-4baa-834b-7fd66d37bc95").ToString(),
            //        OrderDate = DateTime.Now,
            //        State = OrderState.InProcess,
            //        OrderItems = new List<OrderItem>()
            //        {
            //            new OrderItem() { UnitPrice = 12.00m, PictureUrl = "https://fakeimg.pl/370x240/EEEEEE/000/?text=RoslynRedT-Shirt", Quantity = 1, ProductName="Roslyn Red T-Shirt" }
            //        }
            //    },
            //    new Order()
            //    {
            //        Id = Guid.NewGuid().ToString(),
            //        BuyerId = new Guid("ebcbcb4c-b032-4baa-834b-7fd66d37bc95").ToString(),
            //        OrderDate = DateTime.Now,
            //        State = OrderState.Delivered,
            //        OrderItems = new List<OrderItem>()
            //        {
            //            new OrderItem() { UnitPrice = 12.05m, PictureUrl = "https://fakeimg.pl/370x240/EEEEEE/000/?text=RoslynRedT-Shirt", Quantity = 1, ProductName="Roslyn Red T-Shirt" }
            //        }
            //    }
            //};
            #endregion
        }

        async public Task<Order> GetOrder(ApplicationUser user, string Id)
        {
            var context = _httpContextAccesor.HttpContext;
            var token = await context.Authentication.GetTokenAsync("access_token");

            _apiClient = new HttpClient();
            _apiClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var ordersUrl = $"{_remoteServiceBaseUrl}/{Id}";
            var dataString = await _apiClient.GetStringAsync(ordersUrl);
            var response = JsonConvert.DeserializeObject<Order>(dataString);

            return response;
        }

        async public Task<List<Order>> GetMyOrders(ApplicationUser user)
        {
            var context = _httpContextAccesor.HttpContext;
            var token = await context.Authentication.GetTokenAsync("access_token");

            _apiClient = new HttpClient();
            _apiClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var ordersUrl = _remoteServiceBaseUrl;
            var dataString = await _apiClient.GetStringAsync(ordersUrl);
            var response = JsonConvert.DeserializeObject<List<Order>>(dataString);

            return response; 
        }

        public Order MapUserInfoIntoOrder(ApplicationUser user, Order order)
        {
            order.ShippingAddress.City = user.City;
            order.ShippingAddress.Street = user.Street;
            order.ShippingAddress.State = user.State;
            order.ShippingAddress.Country = user.Country;

            order.PaymentInfo.CardNumber = user.CardNumber;
            order.PaymentInfo.CardHolderName = user.CardHolderName;
            order.PaymentInfo.Expiration = user.Expiration;
            order.PaymentInfo.SecurityNumber = user.SecurityNumber;

            return order;
        }

        public OrderRequest MapOrderIntoOrderRequest(Order order)
        {
            var od = new OrderRequest()
            {
                CardHolderName = order.PaymentInfo.CardHolderName,
                CardNumber = order.PaymentInfo.CardNumber,
                CardSecurityNumber = order.PaymentInfo.SecurityNumber,
                CardTypeId = (int)order.PaymentInfo.CardType,
                City = order.ShippingAddress.City,
                Country = order.ShippingAddress.Country,
                State = order.ShippingAddress.State,
                Street = order.ShippingAddress.Street,
                ZipCode = order.ShippingAddress.ZipCode, 
            };

            foreach (var item in order.OrderItems)
            {
                od.Items.Add(new OrderRequestItem()
                {
                    Discount = item.Discount,
                    ProductId = int.Parse(item.ProductId),
                    ProductName = item.ProductName,
                    UnitPrice = item.UnitPrice,
                    Units = item.Quantity
                });           
            }

            return od;
        }

        async public Task CreateOrder(ApplicationUser user, Order order)
        {
            var context = _httpContextAccesor.HttpContext;
            var token = await context.Authentication.GetTokenAsync("access_token");

            _apiClient = new HttpClient();
            _apiClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var ordersUrl = $"{_remoteServiceBaseUrl}/new";
            order.PaymentInfo.CardType = CardType.AMEX;
            OrderRequest request = MapOrderIntoOrderRequest(order);
            StringContent content = new StringContent(JsonConvert.SerializeObject(request), System.Text.Encoding.UTF8, "application/json");
            
            var response = await _apiClient.PostAsync(ordersUrl, content);

            if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                throw new Exception("Error creating order, try later");
        }

        public void OverrideUserInfoIntoOrder(Order original, Order destination)
        {
            destination.ShippingAddress.City = original.ShippingAddress.City;
            destination.ShippingAddress.Street = original.ShippingAddress.Street;
            destination.ShippingAddress.State = original.ShippingAddress.State;
            destination.ShippingAddress.Country = original.ShippingAddress.Country;

            destination.PaymentInfo.CardNumber = original.PaymentInfo.CardNumber;
            destination.PaymentInfo.CardHolderName = original.PaymentInfo.CardHolderName;
            destination.PaymentInfo.Expiration = original.PaymentInfo.Expiration;
            destination.PaymentInfo.SecurityNumber = original.PaymentInfo.SecurityNumber;
        }
    }
}
