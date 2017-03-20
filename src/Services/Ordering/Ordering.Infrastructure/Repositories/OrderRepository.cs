using Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel.OrderAggregate;
using Microsoft.eShopOnContainers.Services.Ordering.Domain.Seedwork;
using System;

namespace Microsoft.eShopOnContainers.Services.Ordering.Infrastructure.Repositories
{
    public class OrderRepository
        : IOrderRepository
    {
        private readonly OrderingContext _context;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public OrderRepository(OrderingContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Order Add(Order order)
        {
            //TODO: when migrating to ef core 1.1.1 change Add by AddAsync-. A bug in ef core 1.1.0 does not allow to do it https://github.com/aspnet/EntityFramework/issues/7298 
            return _context.Orders.Add(order)
                .Entity;
        }
    }
}
