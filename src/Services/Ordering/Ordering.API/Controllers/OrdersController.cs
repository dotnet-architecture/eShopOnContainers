using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Microsoft.eShopOnContainers.Services.Ordering.API.UnitOfWork;
using Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel;


namespace Microsoft.eShopOnContainers.Services.Ordering.API.Controllers
{
    [Route("api/[controller]")]
    public class OrdersController : Controller
    {
        private OrderingContext _context;

        public OrdersController(OrderingContext context)
        {
            //Injected DbContext from the IoC container
            _context = context;
        }

        // GET api/orders
        [HttpGet]
        public IEnumerable<Order> Get()
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
            _context.Orders.Add(order1);
            _context.SaveChanges();

            return _context.Orders.ToList();
        }

        // GET api/orders/xxxGUIDxxxx
        [HttpGet("{id}")]
        public string Get(Guid id)
        {
            return "value TBD";
        }

        // POST api/orders
        [HttpPost]
        public void Post([FromBody]Order order)
        {
            _context.Orders.Add(order);
            _context.SaveChanges();
        }

        // PUT api/orders/xxxGUIDxxxx
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]Order value)
        {

        }

        // DELETE api/orders/xxxGUIDxxxx
        [HttpDelete("{id}")]
        public void Delete(Guid id)
        {

        }

    }

}


