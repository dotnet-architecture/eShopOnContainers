using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel;
using Microsoft.eShopOnContainers.Services.Ordering.Domain.Contracts;
using Microsoft.eShopOnContainers.Services.Ordering.SqlData.UnitOfWork;
using Microsoft.eShopOnContainers.Services.Ordering.Domain.SeedWork;
using Microsoft.EntityFrameworkCore;

namespace Microsoft.eShopOnContainers.Services.Ordering.SqlData.Repositories
{
    //1:1 relationship between Repository and Aggregate (i.e. OrderRepository and Order)
    public class OrderRepository : IOrderRepository
    {
        private readonly OrderingDbContext _context;

        public IUnitOfWork UnitOfWork => _context;

        public OrderRepository(OrderingDbContext orderingDbContext)
        {
            _context = orderingDbContext;
        }

        public void Add(Order order)
        {
            _context.Orders.Add(order);
        }

        public void Update(Order order)
        {
            _context.Orders.Update(order);
        }

        public async Task Remove(Guid orderId)
        {
            var orderToRemove = await _context.Orders.Where(o => o.Id == orderId).SingleOrDefaultAsync();
            _context.Orders.Remove(orderToRemove);
        }

        public async Task<Order> FindAsync(Guid id)
        {
            if (id != Guid.Empty)
                return await _context.Set<Order>().FirstOrDefaultAsync(o => o.Id == id);
            else
                return null;
        }
    }

}
