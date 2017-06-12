using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.eShopOnContainers.BuildingBlocks.Resilience.Http;
using Microsoft.eShopOnContainers.WebMVC.ViewModels;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebMVC.Infrastructure;

namespace Microsoft.eShopOnContainers.WebMVC.Services
{
    public class OrderingService : IOrderingService
    {
        private IHttpClient _apiClient;
        private readonly string _remoteServiceBaseUrl;
        private readonly IOptionsSnapshot<AppSettings> _settings;
        private readonly IHttpContextAccessor _httpContextAccesor;

        public OrderingService(IOptionsSnapshot<AppSettings> settings, IHttpContextAccessor httpContextAccesor, IHttpClient httpClient)
        {
            _remoteServiceBaseUrl = $"{settings.Value.OrderingUrl}/api/v1/orders";
            _settings = settings;
            _httpContextAccesor = httpContextAccesor;
            _apiClient = httpClient;
        }

        async public Task<Order> GetOrder(ApplicationUser user, string id)
        {
            var token = await GetUserTokenAsync();
            var getOrderUri = API.Order.GetOrder(_remoteServiceBaseUrl, id);

            var dataString = await _apiClient.GetStringAsync(getOrderUri, token);

            var response = JsonConvert.DeserializeObject<Order>(dataString);

            return response;
        }

        async public Task<List<Order>> GetMyOrders(ApplicationUser user)
        {
            var token = await GetUserTokenAsync();
            var allMyOrdersUri = API.Order.GetAllMyOrders(_remoteServiceBaseUrl);

            var dataString = await _apiClient.GetStringAsync(allMyOrdersUri, token);
            var response = JsonConvert.DeserializeObject<List<Order>>(dataString);

            return response;
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

        async public Task CreateOrder(Order order)
        {
            var token = await GetUserTokenAsync();
            var requestId = order.RequestId.ToString();
            var addNewOrderUri = API.Order.AddNewOrder(_remoteServiceBaseUrl);

            order.CardTypeId = 1;
            order.CardExpirationApiFormat();

            SetFakeIdToProducts(order);

            var response = await _apiClient.PostAsync(addNewOrderUri, order, token, requestId);

            if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                throw new Exception("Error creating order, try later.");
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

        void SetFakeIdToProducts(Order order)
        {
            var id = 1;
            order.OrderItems.ForEach(x => { x.ProductId = id; id++; });
        }

        async Task<string> GetUserTokenAsync()
        {
            var context = _httpContextAccesor.HttpContext;

            return await context.Authentication.GetTokenAsync("access_token");
        }
    }
}
