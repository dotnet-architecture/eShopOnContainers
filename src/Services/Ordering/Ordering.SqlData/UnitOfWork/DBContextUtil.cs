using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.eShopOnContainers.Services.Ordering.SqlData.UnitOfWork
{
    public class DbContextUtil
    {
        public static DbContextOptions<OrderingDbContext> CreateNewContextOptionsForInMemoryDB()
        {
            // Create a fresh service provider, and therefore a fresh 
            // InMemory database instance.
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            // Create a new options instance telling the context to use an
            // InMemory database and the new service provider.
            var builder = new DbContextOptionsBuilder<OrderingDbContext>();
            builder.UseInMemoryDatabase()
                   .UseInternalServiceProvider(serviceProvider);

            return builder.Options;
        }

        public static DbContextOptions<OrderingDbContext> CreateNewContextOptionsForSqlDb()
        {
            // Create a new options instance telling the context to use a Sql database 
            var builder = new DbContextOptionsBuilder<OrderingDbContext>();

            //SQL LocalDB 
            //var connString = @"Server=(localdb)\mssqllocaldb;Database=Microsoft.eShopOnContainers.Services.OrderingDb;Trusted_Connection=True;";

            //SQL SERVER on-premises

            //(Integrated Security) var connString = @"Server=CESARDLBOOKVHD;Database=Microsoft.eShopOnContainers.Services.OrderingDb;Trusted_Connection=True;";

            //(SQL Server Authentication)
            var connString = @"Server=CESARDLBOOKVHD;Database=Microsoft.eShopOnContainers.Services.OrderingDb;User Id=sa;Password=Pass@word;";

            //SQL LOCALDB
            builder.UseSqlServer(connString);

            return builder.Options;
        }
    }
}
