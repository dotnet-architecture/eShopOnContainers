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
using eShopOnContainers.Core.Models.Basket;
using System.Collections.Generic;
using eShopOnContainers.Core.Services.Basket;
using eShopOnContainers.Core.Services.Order;

namespace eShopOnContainers.Core.ViewModels
{
    public class CheckoutViewModel : ViewModelBase
    {
        private ObservableCollection<BasketItem> _orderItems;
        private Order _order;
        private User _user;

        private IUserService _userService;
        private IBasketService _basketService;
        private IOrderService _orderService;

        public CheckoutViewModel(IUserService userService,
            IBasketService basketService,
            IOrderService orderService)
        {
            _basketService = basketService;
            _userService = userService;
            _orderService = orderService;
        }

        public ObservableCollection<BasketItem> OrderItems
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
            if (navigationData is ObservableCollection<BasketItem>)
            {
                IsBusy = true;

                var orderItems = ((ObservableCollection<BasketItem>)navigationData);

                OrderItems = orderItems;

                User = await _userService.GetUserAsync();

                Order = new Order
                {
                    ShippingAddress = User,
                    OrderItems = CreateOrderItems(orderItems),
                    Status = OrderStatus.Pending,
                    OrderDate = DateTime.Now,
                    Total = GetOrderTotal()
                };

                IsBusy = false;
            }
        }

        private async void Checkout()
        {
            await _orderService.CreateOrderAsync(Order);
            await _basketService.ClearBasketAsync(User.GuidUser);
            
            await NavigationService.NavigateToAsync<MainViewModel>(new TabParameter { TabIndex = 1 });
            await NavigationService.RemoveLastFromBackStackAsync();

            await DialogService.ShowAlertAsync("Order sent successfully!", string.Format("Order {0}", Order.OrderNumber), "Ok");
            await NavigationService.RemoveLastFromBackStackAsync();
        }

        private List<OrderItem> CreateOrderItems(ObservableCollection<BasketItem> basketItems)
        {
            var orderItems = new List<OrderItem>();

            foreach (var basketItem in basketItems)
            {
                orderItems.Add(new OrderItem
                {
                    ProductId = basketItem.ProductId,
                    ProductName = basketItem.ProductName,
                    ProductImage = basketItem.PictureUrl,
                    Quantity = basketItem.Quantity,
                    UnitPrice = basketItem.UnitPrice
                });
            }

            return orderItems;
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