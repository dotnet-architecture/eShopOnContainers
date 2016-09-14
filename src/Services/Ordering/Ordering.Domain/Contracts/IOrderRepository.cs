using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel;
using Microsoft.eShopOnContainers.Services.Ordering.Domain.SeedWork;


namespace Microsoft.eShopOnContainers.Services.Ordering.Domain.Contracts
{
    public interface IOrderRepository : IRepository
    {
        void Add(Order order);
        void Update(Order order);
        Task Remove(Guid id);
        Task<Order> FindAsync(Guid id);
    }
}

