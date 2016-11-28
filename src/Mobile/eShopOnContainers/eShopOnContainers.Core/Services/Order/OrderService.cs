using eShopOnContainers.Core.Services.RequestProvider;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace eShopOnContainers.Core.Services.Order
{
    public class OrderService : IOrderService
    {
        private readonly IRequestProvider _requestProvider;

        public OrderService(IRequestProvider requestProvider)
        {
            _requestProvider = requestProvider;
        }

        public async Task CreateOrderAsync(Models.Orders.Order newOrder)
        {
            UriBuilder builder = new UriBuilder(GlobalSetting.OrdersEndpoint);

            builder.Path = "api/v1/orders/new";

            string uri = builder.ToString();

            await _requestProvider.PostAsync(uri, newOrder);
        }

        public async Task<ObservableCollection<Models.Orders.Order>> GetOrdersAsync()
        {
            try
            {
                UriBuilder builder = new UriBuilder(GlobalSetting.OrdersEndpoint);

                builder.Path = "api/v1/orders";

                string uri = builder.ToString();

                ObservableCollection<Models.Orders.Order> orders =
                    await _requestProvider.GetAsync<ObservableCollection<Models.Orders.Order>>(uri);

                return orders;
            }
            catch
            {
                return new ObservableCollection<Models.Orders.Order>();
            }
        }

        public async Task<Models.Orders.Order> GetOrderAsync(int orderId)
        {
            try
            {
                UriBuilder builder = new UriBuilder(GlobalSetting.OrdersEndpoint);

                builder.Path = string.Format("api/v1/orders/{0}", orderId);

                string uri = builder.ToString();

                Models.Orders.Order order =
                    await _requestProvider.GetAsync<Models.Orders.Order>(uri);

                return order;
            }
            catch
            {
                return new Models.Orders.Order();
            }
        }

        public async Task<ObservableCollection<Models.Orders.CardType>> GetCardTypesAsync()
        {
            try
            {
                UriBuilder builder = new UriBuilder(GlobalSetting.OrdersEndpoint);

                builder.Path = "api/v1/orders/cardtypes";

                string uri = builder.ToString();

                ObservableCollection<Models.Orders.CardType> cardTypes =
                    await _requestProvider.GetAsync<ObservableCollection<Models.Orders.CardType>>(uri);

                return cardTypes;
            }
            catch
            {
                return new ObservableCollection<Models.Orders.CardType>();
            }
        }
    }
}