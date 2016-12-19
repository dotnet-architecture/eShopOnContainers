using eShopOnContainers.Core.Models.Navigation;
using eShopOnContainers.ViewModels.Base;
using System.Windows.Input;
using Xamarin.Forms;
using System.Threading.Tasks;
using eShopOnContainers.Core.Models.Orders;
using System;
using System.Collections.ObjectModel;
using eShopOnContainers.Core.Models.Basket;
using System.Collections.Generic;
using eShopOnContainers.Core.Services.Basket;
using eShopOnContainers.Core.Services.Order;
using eShopOnContainers.Core.Helpers;
using eShopOnContainers.Core.Services.User;
using eShopOnContainers.Core.Models.User;

namespace eShopOnContainers.Core.ViewModels
{
    public class CheckoutViewModel : ViewModelBase
    {
        private ObservableCollection<BasketItem> _orderItems;
        private Order _order;
        private Address _shippingAddress; 

        private IBasketService _basketService;
        private IOrderService _orderService;
        private IUserService _userService;

        public CheckoutViewModel(
            IBasketService basketService,
            IOrderService orderService,
            IUserService userService)
        {
            _basketService = basketService;
            _orderService = orderService;
            _userService = userService;
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

        public Address ShippingAddress
        {
            get { return _shippingAddress; }
            set
            {
                _shippingAddress = value;
                RaisePropertyChanged(() => ShippingAddress);
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

                var authToken = Settings.AuthAccessToken;
                var userInfo = await _userService.GetUserInfoAsync(authToken);

                ShippingAddress = new Address
                {
                    Street = userInfo?.Street,
                    ZipCode = userInfo?.ZipCode,
                    State = userInfo?.State,
                    Country = userInfo?.Country,
                };

                var paymentInfo = new PaymentInfo
                {
                    CardNumber = userInfo?.CardNumber,
                    CardHolderName = userInfo?.CardHolder,
                    SecurityNumber = userInfo?.CardSecurityNumber
                };

                Order = new Order
                {
                    BuyerId = userInfo.UserId,
                    OrderItems = CreateOrderItems(orderItems),
                    State = OrderState.InProcess,
                    OrderDate = DateTime.Now,
                    CardHolderName = paymentInfo.CardHolderName,
                    CardNumber = paymentInfo.CardNumber,
                    CardSecurityNumber = paymentInfo.SecurityNumber,
                    CardExpiration = DateTime.Now.AddYears(5),
                    ShippingState = _shippingAddress.State,
                    ShippingCountry = _shippingAddress.Country,
                    ShippingStreet = _shippingAddress.Street
                };

                IsBusy = false;
            }
        }

        private async void Checkout()
        {
            var authToken = Settings.AuthAccessToken;

            await _orderService.CreateOrderAsync(Order, authToken);

            await _basketService.ClearBasketAsync(_shippingAddress.Id.ToString(), authToken);
            
            await NavigationService.NavigateToAsync<MainViewModel>(new TabParameter { TabIndex = 1 });
            await NavigationService.RemoveLastFromBackStackAsync();

            await DialogService.ShowAlertAsync("Order sent successfully!", string.Format("Order {0}", Order.SequenceNumber), "Ok");
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
                    PictureUrl = basketItem.PictureUrl,
                    Quantity = basketItem.Quantity,
                    UnitPrice = basketItem.UnitPrice
                });
            }

            return orderItems;
        } 
    }
}