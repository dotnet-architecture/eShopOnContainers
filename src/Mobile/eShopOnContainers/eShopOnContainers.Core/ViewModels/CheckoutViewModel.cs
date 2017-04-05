using eShopOnContainers.Core.Models.Navigation;
using eShopOnContainers.Core.ViewModels.Base;
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

        public ICommand CheckoutCommand => new Command(async () => await CheckoutAsync());

        public override async Task InitializeAsync(object navigationData)
        {
            if (navigationData is ObservableCollection<BasketItem>)
            {
                IsBusy = true;

                // Get navigation data
                var orderItems = ((ObservableCollection<BasketItem>)navigationData);

                OrderItems = orderItems;

                var authToken = Settings.AuthAccessToken;       
                var userInfo = await _userService.GetUserInfoAsync(authToken);

                // Create Shipping Address
                ShippingAddress = new Address
                {
                    Id = !string.IsNullOrEmpty(userInfo?.UserId) ? new Guid(userInfo.UserId) : Guid.NewGuid(),
                    Street = userInfo?.Street,
                    ZipCode = userInfo?.ZipCode,
                    State = userInfo?.State,
                    Country = userInfo?.Country,
                    City = userInfo?.Address
                };

                // Create Payment Info
                var paymentInfo = new PaymentInfo
                {
                    CardNumber = userInfo?.CardNumber,
                    CardHolderName = userInfo?.CardHolder,
                    CardType = new CardType {  Id = 3, Name = "MasterCard" },
                    SecurityNumber = userInfo?.CardSecurityNumber
                };

                // Create new Order
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
                    CardTypeId = paymentInfo.CardType.Id,
                    ShippingState = _shippingAddress.State,
                    ShippingCountry = _shippingAddress.Country,
                    ShippingStreet = _shippingAddress.Street,
                    ShippingCity = _shippingAddress.City,  
                    ShippingZipCode = _shippingAddress.ZipCode,
                    Total = CalculateTotal(CreateOrderItems(orderItems))
                };

                IsBusy = false;
            }
        }

        private async Task CheckoutAsync()
        {
            try
            {
                var authToken = Settings.AuthAccessToken;

                // Create new order
                await _orderService.CreateOrderAsync(Order, authToken);

                // Clean Basket
                await _basketService.ClearBasketAsync(_shippingAddress.Id.ToString(), authToken);

                // Reset Basket badge
                var basketViewModel = ViewModelLocator.Resolve<BasketViewModel>();
                basketViewModel.BadgeCount = 0;

                // Navigate to Orders
                await NavigationService.NavigateToAsync<MainViewModel>(new TabParameter { TabIndex = 1 });
                await NavigationService.RemoveLastFromBackStackAsync();

                // Show Dialog
                await DialogService.ShowAlertAsync("Order sent successfully!", string.Format("Order {0}", Order.OrderNumber), "Ok");
                await NavigationService.RemoveLastFromBackStackAsync();
            }
            catch
            {
                await DialogService.ShowAlertAsync("An error ocurred. Please, try again.", "Oops!", "Ok");
            }
        }

        private List<OrderItem> CreateOrderItems(ObservableCollection<BasketItem> basketItems)
        {
            var orderItems = new List<OrderItem>();

            foreach (var basketItem in basketItems)
            {
                if (!string.IsNullOrEmpty(basketItem.ProductName))
                {
                    orderItems.Add(new OrderItem
                    {
                        OrderId = null,
                        ProductId = basketItem.ProductId,
                        ProductName = basketItem.ProductName,
                        PictureUrl = basketItem.PictureUrl,
                        Quantity = basketItem.Quantity,
                        UnitPrice = basketItem.UnitPrice
                    });
                }
            }

            return orderItems;
        } 

        private decimal CalculateTotal(List<OrderItem> orderItems)
        {
            decimal total = 0;

            foreach(var orderItem in orderItems)
            {
                total += (orderItem.Quantity * orderItem.UnitPrice);
            }

            return total;
        }
    }
}