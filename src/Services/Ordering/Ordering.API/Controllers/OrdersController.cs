namespace Microsoft.eShopOnContainers.Services.Ordering.API.Controllers
{
    using Api.Application.Commands;
    using Api.Application.Queries;
    using AspNetCore.Authorization;
    using MediatR;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    [Route("api/v1/[controller]")]
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IOrderQueries _orderQueries;

        public OrdersController(IMediator mediator, IOrderQueries orderQueries)
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
        public async Task<IActionResult> AddOrder([FromBody]NewOrderRequest order)
        {
            if (order.CardExpiration == DateTime.MinValue)
                order.CardExpiration = DateTime.Now.AddYears(5);

            if (order.CardTypeId == 0)
                order.CardTypeId = 1;

            order.Buyer = GetUserName();

            var added = await _mediator.SendAsync(order);
            if (added)
            {
                return Ok();
            }

            return BadRequest();
        }

        [Route("{orderId:int}")]
        [HttpGet]
        public async Task<IActionResult> GetOrder(int orderId)
        {
            try
            {
                var order = await _orderQueries.GetOrder(orderId);
                return Ok(order);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [Route("")]
        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            var orders = await _orderQueries.GetOrders();

            return Ok(orders);
        }

        [Route("cardtypes")]
        [HttpGet]
        public async Task<IActionResult> GetCardTypes()
        {
            var cardTypes = await _orderQueries.GetCardTypes();

            return Ok(cardTypes);
        }

        /// <summary>
        /// Returns the GUID corresponding to the Id of the authenticated user.
        /// </summary>
        /// <returns>GUID (string)</returns>
        string GetUserName()
        {
            return HttpContext.User.FindFirst("sub").Value;
        }
    }

}


