namespace Microsoft.eShopOnContainers.Services.Ordering.Infrastructure.Repositories
{
    using Domain;
    using Domain.SeedWork;
    using Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel.OrderAggregate;
    using System;

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
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            _context = context;
        }

        public Order Add(Order order)
        {
            return _context.Orders.Add(order)
                .Entity;
        }
    }
}
