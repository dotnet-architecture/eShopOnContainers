using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Xunit;
using Microsoft.eShopOnContainers.Services.Ordering.SqlData.UnitOfWork;
using Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DataIntegrationTests
{
    //Basic documentation for Testing EF Core classes
    // http://ef.readthedocs.io/en/latest/miscellaneous/testing.html 
    public class Tests
    {
        private static DbContextOptions<OrderingDbContext> CreateNewContextOptionsForInMemoryDB()
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

        private static DbContextOptions<OrderingDbContext> CreateNewContextOptionsForSqlDB()
        {
            // Create a new options instance telling the context to use a Sql database 
            var builder = new DbContextOptionsBuilder<OrderingDbContext>();

            //SQL LOCALDB
            builder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Microsoft.eShopOnContainers.Services.OrderingDb;Trusted_Connection=True;");

            return builder.Options;
        }

        [Fact]
        public void Add_orders_to_database()
        {
            // All contexts that share the same service provider will share the same database

            //Using InMemory DB
            var options = CreateNewContextOptionsForInMemoryDB();

            //Using Sql LocalDB
            //var options = CreateNewContextOptionsForSqlDB();
            

            // Run the test against one instance of the context
            using (var context = new OrderingDbContext(options))
            {

                //Create generic Address ValueObject
                Address sampleAddress = new Address("15703 NE 61st Ct.",
                                                    "Redmond",
                                                    "Washington",
                                                    "WA",
                                                    "United States",
                                                    "US",
                                                    "98052",
                                                    47.661492,
                                                    -122.131309
                                                    );
                //Create sample Orders
                Order order1 = new Order(Guid.NewGuid(), sampleAddress, sampleAddress);
                context.Orders.Add(order1);
                context.SaveChanges();

                Assert.True(true);
            }

            //// Use a separate instance of the context to verify correct data was saved to database
            using (var context = new OrderingDbContext(options))
            {
                var orders = context.Orders
                                        .Include(o => o.ShippingAddress)
                                        .Include(o => o.BillingAddress)
                                        .ToList();
                                        //Could be using .Load() if you don't want to create a List

                //SAMPLE
                //var company = context.Companies
                //                         .Include(co => co.Employees).ThenInclude(emp => emp.Employee_Car)
                //                         .Include(co => co.Employees).ThenInclude(emp => emp.Employee_Country)
                //                         .FirstOrDefault(co => co.companyID == companyID);

                //Assert when running test with a clean In-Memory DB
                //Assert.Equal(1, context.Orders.Count());

                Assert.Equal("Redmond", orders.First<Order>().ShippingAddress.City);
            }
        }

    }
}
