using Microsoft.eShopOnContainers.WebMVC.ViewModels;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using WebMVC.Infrastructure;
using WebMVC.Services.ModelDTOs;

namespace Microsoft.eShopOnContainers.WebMVC.Services
{
    public class OrderingService : IOrderingService
    {
        private HttpClient _httpClient;
        private readonly string _remoteServiceBaseUrl;
        private readonly IOptions<AppSettings> _settings;


        public OrderingService(HttpClient httpClient, IOptions<AppSettings> settings)
        {
            _httpClient = httpClient;
            _settings = settings;

            _remoteServiceBaseUrl = $"{settings.Value.PurchaseUrl}/o/api/v1/orders";
        }

        async public Task<Order> GetOrder(ApplicationUser user, string id)
        {
            var uri = API.Order.GetOrder(_remoteServiceBaseUrl, id);

            var responseString = await _httpClient.GetStringAsync(uri);

            var response = JsonConvert.DeserializeObject<Order>(responseString);

            return response;
        }

        async public Task<List<Order>> GetMyOrders(ApplicationUser user)
        {
            var uri = API.Order.GetAllMyOrders(_remoteServiceBaseUrl);

            var responseString = await _httpClient.GetStringAsync(uri);

            var response = JsonConvert.DeserializeObject<List<Order>>(responseString);

            return response;
        }



        async public Task CancelOrder(string orderId)
        {
            var order = new OrderDTO()
            {
                OrderNumber = orderId
            };

            var uri = API.Order.CancelOrder(_remoteServiceBaseUrl);
            var orderContent = new StringContent(JsonConvert.SerializeObject(order), System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync(uri, orderContent);

            if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                throw new Exception("Error cancelling order, try later.");
            }

            response.EnsureSuccessStatusCode();
        }

        async public Task ShipOrder(string orderId)
        {
            var order = new OrderDTO()
            {
                OrderNumber = orderId
            };

            var uri = API.Order.ShipOrder(_remoteServiceBaseUrl);
            var orderContent = new StringContent(JsonConvert.SerializeObject(order), System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync(uri, orderContent);

            if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                throw new Exception("Error in ship order process, try later.");
            }

            response.EnsureSuccessStatusCode();
        }

        public void OverrideUserInfoIntoOrder(Order original, Order destination)
        {
            destination.City = original.City;
            destination.Street = original.Street;
            destination.State = original.State;
            destination.Country = original.Country;
            destination.ZipCode = original.ZipCode;

            destination.CardNumber = original.CardNumber;
            destination.CardHolderName = original.CardHolderName;
            destination.CardExpiration = original.CardExpiration;
            destination.CardSecurityNumber = original.CardSecurityNumber;
        }

        public Order MapUserInfoIntoOrder(ApplicationUser user, Order order)
        {
            order.City = user.City;
            order.Street = user.Street;
            order.State = user.State;
            order.Country = user.Country;
            order.ZipCode = user.ZipCode;

            order.CardNumber = user.CardNumber;
            order.CardHolderName = user.CardHolderName;
            order.CardExpiration = new DateTime(int.Parse("20" + user.Expiration.Split('/')[1]), int.Parse(user.Expiration.Split('/')[0]), 1);
            order.CardSecurityNumber = user.SecurityNumber;

            return order;
        }

        public BasketDTO MapOrderToBasket(Order order)
        {
            order.CardExpirationApiFormat();

            return new BasketDTO()
            {
                City = order.City,
                Street = order.Street,
                State = order.State,
                Country = order.Country,
                ZipCode = order.ZipCode,
                CardNumber = order.CardNumber,
                CardHolderName = order.CardHolderName,
                CardExpiration = order.CardExpiration,
                CardSecurityNumber = order.CardSecurityNumber,
                CardTypeId = 1,
                Buyer = order.Buyer,
                RequestId = order.RequestId
            };
        }
    }
}
