using eShopOnContainers.Core.Models.Orders;
using eShopOnContainers.Core.Services.Order;
using eShopOnContainers.Core.Services.Settings;
using eShopOnContainers.Core.ViewModels.Base;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace eShopOnContainers.Core.ViewModels
{
    public class OrderDetailViewModel : ViewModelBase
    {
        private readonly ISettingsService _settingsService;
        private readonly IOrderService _ordersService;

        private Order _order;
        private bool _isSubmittedOrder;
        private string _orderStatusText;

        public OrderDetailViewModel(ISettingsService settingsService, IOrderService ordersService)
        {
            _settingsService = settingsService;
            _ordersService = ordersService;
        }

        public Order Order
        {
            get => _order;
            set
            {
                _order = value;
                RaisePropertyChanged(() => Order);
            }
        }

        public bool IsSubmittedOrder
        {
            get => _isSubmittedOrder;
            set
            {
                _isSubmittedOrder = value;
                RaisePropertyChanged(() => IsSubmittedOrder);
            }
        }

        public string OrderStatusText
        {
            get => _orderStatusText;
            set
            {
                _orderStatusText = value;
                RaisePropertyChanged(() => OrderStatusText);
            }
        }


        public ICommand ToggleCancelOrderCommand => new Command(async () => await ToggleCancelOrderAsync());

        public override async Task InitializeAsync(object navigationData)
        {
            if (navigationData is Order)
            {
                IsBusy = true;

                var order = navigationData as Order;

                // Get order detail info
                var authToken = _settingsService.AuthAccessToken;
                Order = await _ordersService.GetOrderAsync(order.OrderNumber, authToken);
                IsSubmittedOrder = Order.OrderStatus == OrderStatus.Submitted;
                OrderStatusText = Order.OrderStatus.ToString().ToUpper();

                IsBusy = false;
            }
        }

        private async Task ToggleCancelOrderAsync()
        {
            var authToken = _settingsService.AuthAccessToken;

            var result = await _ordersService.CancelOrderAsync(_order.OrderNumber, authToken);

            if (result)
            {
                OrderStatusText = OrderStatus.Cancelled.ToString().ToUpper();
            }
            else
            {
                Order = await _ordersService.GetOrderAsync(Order.OrderNumber, authToken);
                OrderStatusText = Order.OrderStatus.ToString().ToUpper();
            }

            IsSubmittedOrder = false;
        }
    }
}