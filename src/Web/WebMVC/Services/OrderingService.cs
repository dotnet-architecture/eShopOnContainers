using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.eShopOnContainers.WebMVC.Models;
using Microsoft.AspNetCore.Http;

namespace Microsoft.eShopOnContainers.WebMVC.Services
{
    public class OrderingService : IOrderingService
    {
        private List<Order> _orders;
        private int _itemsInCart;
        private IHttpContextAccessor _httpContextAccessor;
        public int ItemsInCart { get { return _itemsInCart; } }
        //var ordersUrl = _settings.OrderingUrl + "/api/ordering/orders";
        //var dataString = await _http.GetStringAsync(ordersUrl);
        //var items = JsonConvert.DeserializeObject<List<Order>>(dataString);

        public OrderingService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;

            _orders = new List<Order>()
            {
                new Order()
                {
                    BuyerId = new Guid("ebcbcb4c-b032-4baa-834b-7fd66d37bc95").ToString(),
                    OrderDate = DateTime.Now,
                    State = OrderState.Active,
                    OrderItems = new List<OrderItem>()
                    {
                        new OrderItem() { UnitPrice = 12, PicsUrl = "https://fakeimg.pl/370x240/EEEEEE/000/?text=RoslynRedT-Shirt", Quantity = 1, ProductName="Roslyn Red T-Shirt" }
                    }
                }, 
                new Order()
                {
                    BuyerId = new Guid("ebcbcb4c-b032-4baa-834b-7fd66d37bc95").ToString(),
                    OrderDate = DateTime.Now,
                    State = OrderState.InProcess,
                    OrderItems = new List<OrderItem>()
                    {
                        new OrderItem() { UnitPrice = 12, PicsUrl = "https://fakeimg.pl/370x240/EEEEEE/000/?text=RoslynRedT-Shirt", Quantity = 1, ProductName="Roslyn Red T-Shirt" }
                    }
                },
                new Order()
                {
                    BuyerId = new Guid("ebcbcb4c-b032-4baa-834b-7fd66d37bc95").ToString(),
                    OrderDate = DateTime.Now,
                    State = OrderState.InProcess,
                    OrderItems = new List<OrderItem>()
                    {
                        new OrderItem() { UnitPrice = 12, PicsUrl = "https://fakeimg.pl/370x240/EEEEEE/000/?text=RoslynRedT-Shirt", Quantity = 1, ProductName="Roslyn Red T-Shirt" }
                    }
                }
            };
        }

        public Order GetActiveOrder(ApplicationUser user)
        {
            //I think could be a good aproach to work with session to store the active order. 
            //....
            Order activeOrder;
            activeOrder = _httpContextAccessor.HttpContext.Session.GetObject<Order>("MyActiveOrder");
            if (activeOrder == null)
                activeOrder = _orders.Where(x => x.BuyerId.Equals(user.Id) && x.State == OrderState.Active).FirstOrDefault();

            _itemsInCart = (activeOrder != null) ? activeOrder.OrderItems.Count() : 0;

            return activeOrder;
        }

        public Order GetOrder(ApplicationUser user, Guid Id)
        {
            return _orders.Where(x => x.BuyerId.Equals(user.Id) && x.Id.Equals(Id)).FirstOrDefault();
        }

        public List<Order> GetMyOrders(ApplicationUser user)
        {
            return _orders.Where(x => x.BuyerId.Equals(user.Id)).ToList();
        }

        public void AddToCart(ApplicationUser user, OrderItem product)
        {
            var activeOrder = GetActiveOrder(user);
            activeOrder.OrderItems.Add(product); 
            //CCE: lacks and httpcall to persist in the real back.end service.
            _httpContextAccessor.HttpContext.Session.SetObject("MyActiveOrder", activeOrder);
            
        }
    }
}
