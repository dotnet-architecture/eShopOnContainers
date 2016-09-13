using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Microsoft.eShopOnContainers.Services.Ordering.SqlData.UnitOfWork;
using Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.eShopOnContainers.Services.Ordering.Domain.RepositoryContracts;
using Microsoft.eShopOnContainers.Services.Ordering.SqlData.Queries;

namespace Microsoft.eShopOnContainers.Services.Ordering.API.Controllers
{
    [Route("api/[controller]")]
    public class OrderingController : Controller
    {
        private IOrderRepository _orderRepository;
        private OrderingQueries _queries;

        public OrderingController(IOrderRepository orderRepository,
                                  OrderingQueries orderingQueries
                                 )
        {
            //Injected objects from the IoC container
            _orderRepository = orderRepository;
            _queries = orderingQueries;
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

        //Alternate method if using several parameters instead of part of the URL
        // GET api/ordering/orders/?orderId=xxxGUIDxxx&otherParam=value
        //[HttpGet("orders")]
        //public Order GetOrderByGuid([FromUri] Guid orderId, [FromUri] string otherParam)


        // POST api/ordering/orders/create
        [HttpPut("orders/create")]
        public async Task<int> Post([FromBody]Order order)
        {
            return await _orderRepository.Add(order);

            //_context.Orders.Add(order);
            //return await _context.SaveChangesAsync();
        }

        // PUT api/ordering/orders/xxxOrderGUIDxxxx/update
        [HttpPut("orders/{orderId:Guid}/update")]
        public async Task<int> UpdateOrder(Guid orderID, [FromBody] Order orderToUpdate)
        {
            return await _orderRepository.Add(orderToUpdate);

            //_context.Orders.Update(orderToUpdate);
            //return await _context.SaveChangesAsync();
        }

        // DELETE api/ordering/orders/xxxOrderGUIDxxxx
        [HttpDelete("orders/{orderId:Guid}/delete")]
        public async Task<int> Delete(Guid id)
        {
            return await _orderRepository.Remove(id);

            //Order orderToDelete = _context.Orders
            //                                     .Where(o => o.Id == id)
            //                                     .SingleOrDefault();

            //_context.Orders.Remove(orderToDelete);
            //return await _context.SaveChangesAsync();
        }

    }

}


