using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Xunit;
using Microsoft.eShopOnContainers.Services.Ordering.SqlData.UnitOfWork;
using Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel;
using Microsoft.eShopOnContainers.Services.Ordering.Domain.RepositoryContracts;
using Microsoft.eShopOnContainers.Services.Ordering.SqlData.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DataIntegrationTests
{
    //Basic documentation for Testing EF Core classes
    // http://ef.readthedocs.io/en/latest/miscellaneous/testing.html 
    public class Tests
    {
        [Fact]
        public void Add_order_to_data_model()
        {
            // All contexts that share the same service provider will share the same database

            //Using InMemory DB
            //var options = DbContextUtil.CreateNewContextOptionsForInMemoryDB();

            //Using Sql LocalDB
            var options = DbContextUtil.CreateNewContextOptionsForSqlDB();

            // Run the test against one instance of the context
            using (var context = new OrderingDbContext(options))
            {
                IOrderRepository orderRepository = new OrderRepository(context);

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

                //Add a few OrderItems
                order1.AddNewOrderItem(Guid.NewGuid(), 2, 25, 30);
                order1.AddNewOrderItem(Guid.NewGuid(), 1, 58, 0);
                order1.AddNewOrderItem(Guid.NewGuid(), 1, 60, 0);
                order1.AddNewOrderItem(Guid.NewGuid(), 3, 12, 0);
                order1.AddNewOrderItem(Guid.NewGuid(), 5, 3, 0);

                orderRepository.Add(order1);

                //With no Async Repository
                //context.Orders.Add(order1);
                //context.SaveChanges();

            }

            //// Use a separate instance of the context to verify correct data was saved to database
            using (var context = new OrderingDbContext(options))
            {
                var orders = context.Orders
                                        .Include(o => o.ShippingAddress)
                                        .Include(o => o.BillingAddress)
                                        .ToList();
                                        //Could be using .Load() if you don't want to create a List

                //OTHER SAMPLE
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
