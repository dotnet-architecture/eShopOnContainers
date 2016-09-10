using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel;

namespace Microsoft.eShopOnContainers.Services.Ordering.API.UnitOfWork
{
    public class OrderingContext : DbContext
    {
        public OrderingContext(DbContextOptions<OrderingContext> options)
            : base(options)
        { }

        public DbSet<Order> Orders { get; set; }
        
    }
}
