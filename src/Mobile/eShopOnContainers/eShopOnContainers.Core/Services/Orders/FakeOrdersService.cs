using eShopOnContainers.Core.Models.Orders;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace eShopOnContainers.Core.Services.Orders
{
    public class FakeOrdersService : IOrdersService
    {
        public async Task<ObservableCollection<Order>> GetOrdersAsync()
        {
            await Task.Delay(500);

            return new ObservableCollection<Order>
            {
                new Order { OrderNumber = 0123456789, Total = 45.30, Date = DateTime.Now, Status = OrderStatus.Delivered },
                new Order { OrderNumber = 9123456780, Total = 39.95, Date = DateTime.Now, Status = OrderStatus.Delivered },
                new Order { OrderNumber = 8765432190, Total = 15.00, Date = DateTime.Now, Status = OrderStatus.Delivered },
            };
        }

        public async Task<Order> GetCartAsync()
        {
            await Task.Delay(500);

            return new Order { OrderNumber = 0123456789, Total = 45.99, Date = DateTime.Now, Status = OrderStatus.Pending };
        }
    }
}