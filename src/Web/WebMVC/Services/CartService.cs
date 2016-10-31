using System;
using System.Threading.Tasks;
using Microsoft.eShopOnContainers.WebMVC.Models;

namespace Microsoft.eShopOnContainers.WebMVC.Services
{
    public class CartService : ICartService
    {
        Order _order;

        public CartService()
        {
            _order = new Order();
            _order.OrderItems = new System.Collections.Generic.List<OrderItem>();
            _order.OrderItems.Add(new OrderItem()
            {
                ProductName = "Cart product"
            });
        }

        public void AddItemToOrder(CatalogItem item)
        {
            throw new NotImplementedException();
        }

        public int GetItemCountFromOrderInProgress()
        {
            throw new NotImplementedException();
        }

        public Task<Order> GetActiveOrder()
        {
            return Task.Run(() => { return _order; });
        }

        public void RemoveItemFromOrder(Guid itemIdentifier)
        {
            throw new NotImplementedException();
        }
    }
}
