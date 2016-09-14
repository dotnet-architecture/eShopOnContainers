using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Microsoft.eShopOnContainers.Services.Ordering.SqlData.UnitOfWork;
using Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.eShopOnContainers.Services.Ordering.Domain.Contracts;
using Microsoft.eShopOnContainers.Services.Ordering.SqlData.Queries;

namespace Microsoft.eShopOnContainers.Services.Ordering.API.Controllers
{
    [Route("api/[controller]")]
    public class OrderingController : Controller
    {
        private IOrderRepository _orderRepository;
        private IOrderdingQueries _queries;

        //private OrderingDbContext _context;

        public OrderingController(IOrderRepository orderRepository,
                                  IOrderdingQueries orderingQueries //,
                                  //OrderingDbContext context
                                 )
        {
            //Injected objects from the IoC container
            _orderRepository = orderRepository;
            _queries = orderingQueries;
            //_context = context;
        }


        // GET api/ordering/orders
        [HttpGet("orders")]
        public async Task<IActionResult> GetAllOrders()
        {
            dynamic response = await _queries.GetAllOrdersIncludingValueObjectsAndChildEntities();
            return Ok(response);
        }

        
        // GET api/ordering/orders/xxxGUIDxxxx
        [HttpGet("orders/{orderId:Guid}")]
        public async Task<IActionResult> GetOrderById(Guid orderId)
        {
            dynamic response = await _queries.GetOrderById(orderId);
            return Ok(response);
        }

        //(CDLTLL) - Using parameters
        //Alternate method if using several parameters instead of part of the URL
        // GET api/ordering/orders/?orderId=xxxGUIDxxx&otherParam=value
        //[HttpGet("orders")]
        //public Order GetOrderByGuid([FromUri] Guid orderId, [FromUri] string otherParam)


        // POST api/ordering/orders/create
        [HttpPut("orders/create")]
        public async Task<IActionResult> Post([FromBody]Order order)
        {
            _orderRepository.Add(order);
            int numChanges = await _orderRepository.UnitOfWork.CommitAsync();
            return Ok(numChanges);
        }

        // PUT api/ordering/orders/xxxOrderGUIDxxxx/update
        [HttpPut("orders/{orderId:Guid}/update")]
        public async Task<IActionResult> UpdateOrder(Guid orderID, [FromBody] Order orderToUpdate)
        {
            _orderRepository.Update(orderToUpdate);
            int numChanges = await _orderRepository.UnitOfWork.CommitAsync();
            return Ok(numChanges);
        }

        // DELETE api/ordering/orders/xxxOrderGUIDxxxx
        [HttpDelete("orders/{orderId:Guid}/remove")]
        public async Task<IActionResult> Remove(Guid id)
        {
            await _orderRepository.Remove(id);
            int numChanges = await _orderRepository.UnitOfWork.CommitAsync();
            return Ok(numChanges);
        }


        // GET api/ordering/orders/add_test_data_and_get_all
        [HttpGet("orders/add_test_data_and_get_all")]
        public async Task<IActionResult> AddTestDataAndGetAllOrders()
        {
            //TEST ADDING ORDERS *********************************
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

            _orderRepository.Add(order1);
            int numRecs = await _orderRepository.UnitOfWork.CommitAsync();

            //_context.Orders.Add(order1);
            //_context.SaveChanges();

            //*****************************************************

            dynamic response = await _queries.GetAllOrdersIncludingValueObjectsAndChildEntities();
            return Ok(response);
        }

    }

}


