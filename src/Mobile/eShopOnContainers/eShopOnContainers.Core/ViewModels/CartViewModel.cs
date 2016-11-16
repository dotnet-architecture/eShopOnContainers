using eShopOnContainers.Core.Models.Catalog;
using eShopOnContainers.Core.Models.Orders;
using eShopOnContainers.Core.Services.Orders;
using eShopOnContainers.Core.ViewModels.Base;
using eShopOnContainers.ViewModels.Base;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace eShopOnContainers.Core.ViewModels
{
    public class CartViewModel : ViewModelBase
    {
        private int _badgeCount;
        private ObservableCollection<OrderItem> _orderItems;
        private decimal _total;

        private IOrdersService _orderService;

        public CartViewModel(IOrdersService orderService)
        {
            _orderService = orderService;
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

        public ObservableCollection<OrderItem> OrderItems
        {
            get { return _orderItems; }
            set
            {
                _orderItems = value;
                RaisePropertyChanged(() => OrderItems);
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

        public override Task InitializeAsync(object navigationData)
        {
            MessagingCenter.Subscribe<CatalogViewModel, CatalogItem>(this, MessengerKeys.AddProduct, (sender, arg) =>
            {
                BadgeCount++;

                AddCartItem(arg);
            });

            OrderItems = new ObservableCollection<OrderItem>();

            return base.InitializeAsync(navigationData);
        }

        private void AddCartItem(CatalogItem item)
        {
            if (OrderItems.Any(o => o.ProductId == Convert.ToInt32(item.Id)))
            {
                var orderItem = OrderItems.First(o => o.ProductId == Convert.ToInt32(item.Id));
                orderItem.Quantity++;
            }
            else
            {
                OrderItems.Add(new OrderItem
                {
                    ProductId = Convert.ToInt32(item.Id),
                    ProductName = item.Name,
                    ProductImage = item.PictureUri,
                    UnitPrice = item.Price,
                    Quantity = 1
                });
            }

            ReCalculateTotal();
        }

        private void ReCalculateTotal()
        {
            Total = 0;

            foreach (var orderItem in OrderItems)
            {
                Total += orderItem.Total;
            }
        }
    }
}
