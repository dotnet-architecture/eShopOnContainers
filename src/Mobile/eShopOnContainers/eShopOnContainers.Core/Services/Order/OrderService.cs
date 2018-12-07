using eShopOnContainers.Core.Helpers;
using eShopOnContainers.Core.Models.Basket;
using eShopOnContainers.Core.Models.Orders;
using eShopOnContainers.Core.Services.RequestProvider;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace eShopOnContainers.Core.Services.Order
{
    public class OrderService : IOrderService
    {
        private readonly IRequestProvider _requestProvider;

        private const string ApiUrlBase = "api/v1/o/orders";

        public OrderService(IRequestProvider requestProvider)
        {
            _requestProvider = requestProvider;
        }

        public Task CreateOrderAsync(Models.Orders.Order newOrder, string token)
        {
            throw new Exception("Only available in Mock Services!");
        }

        public async Task<ObservableCollection<Models.Orders.Order>> GetOrdersAsync(string token)
        {
            var uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewayShoppingEndpoint, ApiUrlBase);

            ObservableCollection<Models.Orders.Order> orders =
                await _requestProvider.GetAsync<ObservableCollection<Models.Orders.Order>>(uri, token);

            return orders;

        }

        public async Task<Models.Orders.Order> GetOrderAsync(int orderId, string token)
        {
            try
            {
                var uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewayShoppingEndpoint, $"{ApiUrlBase}/{orderId}");

                Models.Orders.Order order =
                    await _requestProvider.GetAsync<Models.Orders.Order>(uri, token);

                return order;
            }
            catch
            {
                return new Models.Orders.Order();
            }
        }

        public BasketCheckout MapOrderToBasket(Models.Orders.Order order)
        {
            return new BasketCheckout()
            {
                CardExpiration = order.CardExpiration,
                CardHolderName = order.CardHolderName,
                CardNumber = order.CardNumber,
                CardSecurityNumber = order.CardSecurityNumber,
                CardTypeId = order.CardTypeId,
                City = order.ShippingCity,
                State = order.ShippingState,
                Country = order.ShippingCountry,
                ZipCode = order.ShippingZipCode,
                Street = order.ShippingStreet
            };
        }

        public async Task<bool> CancelOrderAsync(int orderId, string token)
        {
            var uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewayShoppingEndpoint, $"{ApiUrlBase}/cancel");

            var cancelOrderCommand = new CancelOrderCommand(orderId);

            var header = "x-requestid";

            try
            {
                await _requestProvider.PutAsync(uri, cancelOrderCommand, token, header);
            }
            //If the status of the order has changed before to click cancel button, we will get
            //a BadRequest HttpStatus
            catch (HttpRequestExceptionEx ex) when (ex.HttpCode == System.Net.HttpStatusCode.BadRequest)
            {
                return false;
            }

            return true;
        }
    }
}