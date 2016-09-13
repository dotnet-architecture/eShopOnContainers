
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.eShopOnContainers.Services.Ordering.SqlData.UnitOfWork;
using Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel;

namespace Microsoft.eShopOnContainers.Services.Ordering.SqlData.Queries
{
    public class OrderingQueries
    {
        private OrderingDbContext _dbContext;

        public OrderingQueries(OrderingDbContext orderingDbContext)
        {
            _dbContext = orderingDbContext;
        }

        public async Task<dynamic> GetAllOrdersIncludingValueObjectsAndChildEntities()
        {
            var orders = await _dbContext.Orders
                                    .Include(o => o.ShippingAddress)
                                    .Include(o => o.BillingAddress)
                                    //.Include(o => o.Items)
                                    .ToListAsync<Order>();

            // Dynamically generated a Response-Model that includes only the fields you need in the response. 
            // This keeps the JSON response minimal.
            // Could also use var
            dynamic response = orders.Select(o => new
            {
                id = o.Id,
                orderDate = o.OrderDate,
                shippingAddress = o.ShippingAddress,
                billingAddress = o.BillingAddress,
                //items = o.Items.Select(i => i.Content)
            });

            return response;
        }
    }
}
