using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.eShopOnContainers.WebMVC.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Net.Http;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.eShopOnContainers.BuildingBlocks.Resilience.Http;

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

        async public Task<Order> GetOrder(ApplicationUser user, string Id)
        {
            var context = _httpContextAccesor.HttpContext;
            var token = await context.Authentication.GetTokenAsync("access_token");
            _apiClient.Inst.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var ordersUrl = $"{_remoteServiceBaseUrl}/{Id}";
            var dataString = await _apiClient.GetStringAsync(ordersUrl);
            
            var response = JsonConvert.DeserializeObject<Order>(dataString);

            return response;
        }

        async public Task<List<Order>> GetMyOrders(ApplicationUser user)
        {
            var context = _httpContextAccesor.HttpContext;
            var token = await context.Authentication.GetTokenAsync("access_token");

            _apiClient.Inst.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var ordersUrl = _remoteServiceBaseUrl;
            var dataString = await _apiClient.GetStringAsync(ordersUrl);
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
            order.CardExpiration = new DateTime(int.Parse("20" + user.Expiration.Split('/')[1]),int.Parse(user.Expiration.Split('/')[0]), 1);
            order.CardSecurityNumber = user.SecurityNumber;

            return order;
        }

        async public Task CreateOrder(Order order)
        {
            var context = _httpContextAccesor.HttpContext;
            var token = await context.Authentication.GetTokenAsync("access_token");

            _apiClient.Inst.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            _apiClient.Inst.DefaultRequestHeaders.Add("x-requestid", order.RequestId.ToString());

            var ordersUrl = $"{_remoteServiceBaseUrl}/new";
            order.CardTypeId = 1;
            order.CardExpirationApiFormat();
            SetFakeIdToProducts(order);

            var response = await _apiClient.PostAsync(ordersUrl, order);

            if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                throw new Exception("Error creating order, try later");

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

        private void SetFakeIdToProducts(Order order)
        {
            var id = 1;
            order.OrderItems.ForEach(x => { x.ProductId = id; id++; });
        }
    }
}
