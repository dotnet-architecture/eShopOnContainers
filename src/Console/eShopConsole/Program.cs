using System;
using System.Linq;
using Microsoft.eShopOnContainers.Services.Ordering.SqlData.UnitOfWork;
using Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel;
using Microsoft.eShopOnContainers.Services.Ordering.Domain.Contracts;
using Microsoft.eShopOnContainers.Services.Ordering.SqlData.Repositories;
using Microsoft.EntityFrameworkCore;

namespace eShopConsole
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // All contexts that share the same service provider will share the same database

            //Using InMemory DB
            //var options = DbContextUtil.CreateNewContextOptionsForInMemoryDB();

            //Using Sql Server
            var options = DbContextUtil.CreateNewContextOptionsForSqlDb();

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
                orderRepository.UnitOfWork.CommitAsync();

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

                string cityName = orders.First<Order>().ShippingAddress.City;
                Console.WriteLine(cityName);
            }
        }
    }
}
