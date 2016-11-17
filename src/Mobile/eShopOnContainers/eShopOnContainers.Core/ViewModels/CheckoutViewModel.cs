using eShopOnContainers.Core.Models.Navigation;
using eShopOnContainers.Core.Services.User;
using eShopOnContainers.ViewModels.Base;
using System.Windows.Input;
using Xamarin.Forms;
using System.Threading.Tasks;
using eShopOnContainers.Core.Models.User;
using eShopOnContainers.Core.Models.Orders;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace eShopOnContainers.Core.ViewModels
{
    public class CheckoutViewModel : ViewModelBase
    {
        private ObservableCollection<OrderItem> _orderItems;
        private Order _order;
        private User _user;

        private IUserService _userService;

        public CheckoutViewModel(IUserService userService)
        {
            _userService = userService;
        }

        public ObservableCollection<OrderItem> OrderItems
        {
            get { return _orderItems; }
            set
            {
                _orderItems = value;
                RaisePropertyChanged(() => OrderItems);
            }
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

        public User User
        {
            get { return _user; }
            set
            {
                _user = value;
                RaisePropertyChanged(() => User);
            }
        }

        public ICommand CheckoutCommand => new Command(Checkout);

        public override async Task InitializeAsync(object navigationData)
        {
            if (navigationData is ObservableCollection<OrderItem>)
            {
                IsBusy = true;

                var orderItems = ((ObservableCollection<OrderItem>)navigationData);

                OrderItems = orderItems;

                User = await _userService.GetUserAsync();

                Order = new Order
                {
                    ShippingAddress = User,
                    OrderItems = orderItems.ToList(),
                    Status = OrderStatus.Pending,
                    OrderDate = DateTime.Now,
                    Total = GetOrderTotal()
                };

                IsBusy = false;
            }
        }

        private async void Checkout()
        {
            await NavigationService.NavigateToAsync<MainViewModel>(new TabParameter { TabIndex = 1 });
            await NavigationService.RemoveLastFromBackStackAsync();

            await DialogService.ShowAlertAsync("Order sent successfully", string.Format("Order {0}", Order.OrderNumber), "Ok");
            await NavigationService.RemoveLastFromBackStackAsync();
        }

        private decimal GetOrderTotal()
        {
            decimal total = 0;

            foreach (var orderItem in OrderItems)
            {
                total += orderItem.Total;
            }

            return total;
        }
    }
}
