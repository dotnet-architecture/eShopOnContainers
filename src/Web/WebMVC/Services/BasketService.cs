using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.eShopOnContainers.WebMVC.Models;
using Microsoft.AspNetCore.Http;

namespace Microsoft.eShopOnContainers.WebMVC.Services
{
    public class BasketService : IBasketService
    {
        private int _itemsInCart;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BasketService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int ItemsInCart { get { return _itemsInCart; }}

        public Basket GetBasket(ApplicationUser user)
        {
            Basket activeOrder;
            activeOrder = _httpContextAccessor.HttpContext.Session.GetObject<Basket>("MyActiveOrder");
              _itemsInCart = (activeOrder != null) ? activeOrder.Items.Count() : 0;

            return activeOrder;
        }

        public Basket UpdateBasket(Basket basket)
        {
            _httpContextAccessor.HttpContext.Session.SetObject("MyActiveOrder", basket);
            return _httpContextAccessor.HttpContext.Session.GetObject<Basket>("MyActiveOrder");
        }

        public Basket SetQuantities(ApplicationUser user, Dictionary<string, int> quantities)
        {
            var basket = GetBasket(user);

            basket.Items.ForEach(x =>
            {
                var quantity = quantities.Where(y => y.Key == x.Id).FirstOrDefault();
                if (quantities.Where(y => y.Key == x.Id).Count() > 0)
                    x.Quantity = quantity.Value;
            });

            return basket;
        }

        public Order MapBasketToOrder(Basket basket)
        {
            var order = new Order()
            {
                Id = Guid.NewGuid().ToString(),
                BuyerId = basket.BuyerId
            };

            basket.Items.ForEach(x =>
            {
                order.OrderItems.Add(new OrderItem()
                {
                    ProductId = x.ProductId,
                    OrderId = order.Id,
                    PictureUrl = x.PictureUrl,
                    ProductName = x.ProductName,
                    Quantity = x.Quantity,
                    UnitPrice = x.UnitPrice
                });
            });

            return order;
        }

        public void AddItemToBasket(ApplicationUser user, BasketItem product)
        {
            Basket activeOrder = GetBasket(user);
            if (activeOrder == null)
            {
                activeOrder = new Basket()
                {
                    BuyerId = user.Id,
                    Id = Guid.NewGuid().ToString(),
                    Items = new List<Models.BasketItem>()
                };
            }

            activeOrder.Items.Add(product);
            //CCE: lacks and httpcall to persist in the real back.end service.
            _httpContextAccessor.HttpContext.Session.SetObject("MyActiveOrder", activeOrder);
        }

        public void CleanBasket(ApplicationUser user)
        {
            _httpContextAccessor.HttpContext.Session.SetObject("MyActiveOrder", new Basket());
        }
    }
}
