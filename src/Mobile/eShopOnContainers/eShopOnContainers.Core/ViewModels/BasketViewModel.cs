using eShopOnContainers.Core.Helpers;
using eShopOnContainers.Core.Models.Basket;
using eShopOnContainers.Core.Models.Catalog;
using eShopOnContainers.Core.Services.Basket;
using eShopOnContainers.Core.Services.User;
using eShopOnContainers.Core.ViewModels.Base;
using eShopOnContainers.ViewModels.Base;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace eShopOnContainers.Core.ViewModels
{
    public class BasketViewModel : ViewModelBase
    {
        private int _badgeCount;
        private ObservableCollection<BasketItem> _basketItems;
        private decimal _total;

        private IBasketService _basketService;
        private IUserService _userService;

        public BasketViewModel(
            IBasketService basketService,
            IUserService userService)
        {
            _basketService = basketService;
            _userService = userService;
        }

        public int BadgeCount
        {
            get { return _badgeCount; }
            set
            {
                _badgeCount = value;
                RaisePropertyChanged(() => BadgeCount);
            }
        }

        public ObservableCollection<BasketItem> BasketItems
        {
            get { return _basketItems; }
            set
            {
                _basketItems = value;
                RaisePropertyChanged(() => BasketItems);
            }
        }
         
        public decimal Total
        {
            get { return _total; }
            set
            {
                _total = value;
                RaisePropertyChanged(() => Total);
            }
        }

        public ICommand AddCommand => new Command<BasketItem>(AddItem);

        public ICommand CheckoutCommand => new Command(Checkout);

        public override async Task InitializeAsync(object navigationData)
        {
            if (BasketItems == null)
                BasketItems = new ObservableCollection<BasketItem>();

            var authToken = Settings.AuthAccessToken;
            var userInfo = await _userService.GetUserInfoAsync(authToken);

            // Update Basket
            var basket = await _basketService.GetBasketAsync(userInfo.UserId, authToken);

            if (basket != null && basket.Items != null && basket.Items.Any())
            {
                BasketItems.Clear();
                foreach (var basketItem in basket.Items)
                {
                    BadgeCount += basketItem.Quantity;
                    AddBasketItem(basketItem);
                }
            }

            MessagingCenter.Unsubscribe<CatalogViewModel, CatalogItem>(this, MessengerKeys.AddProduct);
            MessagingCenter.Subscribe<CatalogViewModel, CatalogItem>(this, MessengerKeys.AddProduct, (sender, arg) =>
            {
                BadgeCount++;

                AddCatalogItem(arg);
            });
            
            await base.InitializeAsync(navigationData);
        }

        private void AddCatalogItem(CatalogItem item)
        {
            BasketItems.Add(new BasketItem
            {
                ProductId = item.Id,
                ProductName = item.Name,
                PictureUrl = item.PictureUri,
                UnitPrice = item.Price,
                Quantity = 1
            });

            ReCalculateTotal();
        }

        private void AddItem(BasketItem item)
        {
            BadgeCount++;

            AddBasketItem(item);

            RaisePropertyChanged(() => BasketItems);
        }

        private void AddBasketItem(BasketItem item)
        {
            BasketItems.Add(item);

            ReCalculateTotal();
        }

        private async void ReCalculateTotal()
        {
            Total = 0;

            if (BasketItems == null)
            {
                return;
            }

            foreach (var orderItem in BasketItems)
            {
                Total += (orderItem.Quantity * orderItem.UnitPrice);
            }

            var authToken = Settings.AuthAccessToken;
            var userInfo = await _userService.GetUserInfoAsync(authToken);

            await _basketService.UpdateBasketAsync(new CustomerBasket
            {
                BuyerId = userInfo.UserId, 
                Items = BasketItems.ToList()
            }, authToken);
        }

        private void Checkout()
        {
            if (BasketItems.Any())
            {
                NavigationService.NavigateToAsync<CheckoutViewModel>(BasketItems);
            }
        }
    }
}