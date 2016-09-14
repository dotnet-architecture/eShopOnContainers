
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.eShopOnContainers.Services.Ordering.SqlData.UnitOfWork;
using Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel;
using Microsoft.eShopOnContainers.Services.Ordering.SqlData.Queries;

namespace Microsoft.eShopOnContainers.Services.Ordering.SqlData.Queries
{
    //In this case, for the Application queries, we're using the same EF Context but another good approach
    //is also to simply use SQL sentences for the queries with any Micro-ORM (like Dapper) or even just ADO.NET
    //
    //The point is that Queries are IDEMPOTENT and don't need to commit to DDD Domain restrictions 
    //so could be implemented in a completely orthogonal way in regards the Domain Layer (à la CQRS)

    public class OrderingQueries : IOrderdingQueries
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
                                    .Include(o => o.OrderItems)
                                    .ToListAsync<Order>();

            // Dynamically generated a Response-Model that includes only the fields you need in the response. 
            // This keeps the JSON response minimal.
            // Could also use var
            dynamic response = orders.Select(o => new
            {
                id = o.Id,
                orderNumber = o.OrderNumber,
                buyerId = o.BuyerId,
                orderDate = o.OrderDate,
                status = o.Status,
                shippingAddress = o.ShippingAddress,
                billingAddress = o.BillingAddress,
                orderItems = o.OrderItems.Select(i => new
                                                      {
                                                        id = i.Id,
                                                        productId = i.ProductId,
                                                        unitPrice = i.UnitPrice,
                                                        quantity = i.Quantity,
                                                        discount = i.Discount
                                                      }
                                                )
            });

            return response;
        }

        public async Task<dynamic> GetOrderById(Guid orderId)
        {
            var order = await _dbContext.Orders
                                            .Include(o => o.ShippingAddress)
                                            .Include(o => o.BillingAddress)
                                            .Include(o => o.OrderItems)
                                            .Where(o => o.Id == orderId)
                                            .SingleOrDefaultAsync<Order>();

            // Dynamically generated a Response-Model that includes only the fields you need in the response. 
            // This keeps the JSON response minimal.
            // Could also use var
            dynamic response = new
            {
                id = order.Id,
                orderNumber = order.OrderNumber,
                buyerId = order.BuyerId,
                orderDate = order.OrderDate,
                status = order.Status,
                shippingAddress = order.ShippingAddress,
                billingAddress = order.BillingAddress,
                orderItems = order.OrderItems.Select(i => new
                                                            {
                                                                id = i.Id,
                                                                productId = i.ProductId,
                                                                unitPrice = i.UnitPrice,
                                                                quantity = i.Quantity,
                                                                discount = i.Discount
                                                            }
                                                     )
            };

            return response;
        }
    }
}
