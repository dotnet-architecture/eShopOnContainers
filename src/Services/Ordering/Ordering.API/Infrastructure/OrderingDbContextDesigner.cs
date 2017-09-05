using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.eShopOnContainers.Services.Ordering.Infrastructure;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Microsoft.eShopOnContainers.Services.Ordering.API.Infrastructure
{
    public class OrderingDbContextDesigner : IDesignTimeDbContextFactory<OrderingContext>
    {

 
        public OrderingContext CreateDbContext(string[] args)
        {
            var options = new DbContextOptionsBuilder<OrderingContext>();
            options.UseSqlServer("Server=tcp:127.0.0.1,5433;Database=Microsoft.eShopOnContainers.Services.OrderingDb;User Id=sa;Password=Pass@word;",
                sqlServerOptionsAction: sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                });
            return OrderingContext.CreateForEFDesignTools(options.Options);
        }

    }
}
