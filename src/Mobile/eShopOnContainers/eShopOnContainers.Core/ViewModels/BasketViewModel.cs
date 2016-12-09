using eShopOnContainers.Core.Helpers;
using eShopOnContainers.Core.Models.Basket;
using eShopOnContainers.Core.Models.Catalog;
using eShopOnContainers.Core.Models.User;
using eShopOnContainers.Core.Services.Basket;
using eShopOnContainers.Core.Services.User;
using eShopOnContainers.Core.ViewModels.Base;
using eShopOnContainers.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace eShopOnContainers.Core.ViewModels
{
    public class BasketViewModel : ViewModelBase
    {
        private User _user;
        private int _badgeCount;
        private ObservableCollection<BasketItem> _basketItems;
        private decimal _total;

        private IUserService _userService;
        private IBasketService _basketService;

        public BasketViewModel(IUserService userService,
            IBasketService basketService)
        {
            _userService = userService;
            _basketService = basketService;
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

        public ICommand CheckoutCommand => new Command(Checkout);

        public override async Task InitializeAsync(object navigationData)
        {
            MessagingCenter.Subscribe<CatalogViewModel, List<BasketItem>>(this, MessengerKeys.UpdateBasket, (sender, arg) =>
            {
                foreach (var basketItem in arg)
                {
                    BadgeCount += basketItem.Quantity;
                    AddBasketItem(basketItem);
                }
            });

            MessagingCenter.Subscribe<CatalogViewModel, CatalogItem>(this, MessengerKeys.AddProduct, (sender, arg) =>
            {
                BadgeCount++;

                AddCatalogItem(arg);
            });
            
            MessagingCenter.Subscribe<BasketItem>(this, MessengerKeys.UpdateProduct, (sender) =>
            {
                ReCalculateTotal();
            });

            _user = await _userService.GetUserAsync();
            BasketItems = new ObservableCollection<BasketItem>();
        }

        private void AddCatalogItem(CatalogItem item)
        {
            if (BasketItems.Any(o => o.ProductId.Equals(item.Id, StringComparison.CurrentCultureIgnoreCase)))
            {
                var orderItem = BasketItems.First(o => o.ProductId.Equals(item.Id, StringComparison.CurrentCultureIgnoreCase));
                orderItem.Quantity++;
            }
            else
            {
                BasketItems.Add(new BasketItem
                {
                    ProductId = item.Id,
                    ProductName = item.Name,
                    PictureUrl = item.PictureUri,
                    UnitPrice = item.Price,
                    Quantity = 1
                });
            }

            ReCalculateTotal();
        }

        private void AddBasketItem(BasketItem item)
        {
            BasketItems.Add(item);

            ReCalculateTotal();
        }

        private void ReCalculateTotal()
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

            _basketService.UpdateBasketAsync(new CustomerBasket
            {
                BuyerId = _user.GuidUser,
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