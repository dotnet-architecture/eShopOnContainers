using System.Threading.Tasks;
using eShopOnContainers.Core.Models.Orders;
using eShopOnContainers.Core.ViewModels.Base;
using eShopOnContainers.Core.Services.Order;
using System;
using eShopOnContainers.Core.Helpers;

namespace eShopOnContainers.Core.ViewModels
{
    public class OrderDetailViewModel : ViewModelBase
    {
        private IOrderService _ordersService;
		private Order _order;

        public OrderDetailViewModel(IOrderService ordersService)
        {
            _ordersService = ordersService;
        }

        public Order Order
        {
            get { return _order; }
            set
            {
                _order = value;
                RaisePropertyChanged(() => Order);
            }
        }

        public override async Task InitializeAsync(object navigationData)
        {
            if (navigationData is Order)
            {
                IsBusy = true;

                var order = navigationData as Order;

                // Get order detail info
                var authToken = Settings.AuthAccessToken;
                Order = await _ordersService.GetOrderAsync(Convert.ToInt32(order.OrderNumber), authToken);

                IsBusy = false;
            }
        }
    }
}