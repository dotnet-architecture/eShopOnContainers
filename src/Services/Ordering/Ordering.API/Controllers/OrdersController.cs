namespace Microsoft.eShopOnContainers.Services.Ordering.API.Controllers
{
    using Application.Commands;
    using Application.Queries;
    using MediatR;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Threading.Tasks;

    [Route("api/v1/[controller]")]
    public class OrdersController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IOrderQueries _orderQueries;

        public OrdersController(IMediator mediator,IOrderQueries orderQueries)
        {
            if (mediator == null)
            {
                throw new ArgumentNullException(nameof(mediator));
            }

            if (orderQueries == null)
            {
                throw new ArgumentNullException(nameof(orderQueries));
            }

            _mediator = mediator;
            _orderQueries = orderQueries;
        }

        [Route("new")]
        [HttpPost]
        public async Task<IActionResult> AddOrder()
        {
            var newOrderRequest = new NewOrderRequest();

            var added = await _mediator.SendAsync(newOrderRequest);

            if (added)
            {
                return Ok();
            }

            return BadRequest();
        }


        [Route("cancel/{orderId:int}")]
        [HttpPost]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            var cancelOrderRequest = new CancelOrderRequest(orderId);

            var cancelled = await _mediator.SendAsync(cancelOrderRequest);

            if (cancelled)
            {
                return Ok();
            }

            return BadRequest();
        }


        [Route("{orderId:int}")]
        [HttpGet]
        public async Task<IActionResult> GetOrder(int orderId)
        {
            var order = await _orderQueries.GetOrder(orderId);

            if ( order != null)
            {
                Ok(order);
            }

            return NotFound();
        }

        [Route("pending")]
        [HttpGet]
        public async Task<IActionResult> GetPendingOrders(int orderId)
        {
            var orders = await _orderQueries.GetPendingOrders();

            if (orders.Any())
            {
                Ok(orders);
            }

            return NoContent();
        }
    }

}


