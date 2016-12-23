namespace Microsoft.eShopOnContainers.Services.Ordering.API.Controllers
{
    using Api.Application.Commands;
    using Api.Application.Queries;
    using AspNetCore.Authorization;
    using Infrastructure.Services;
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
        private readonly IIdentityService _identityService;

        public OrdersController(IMediator mediator, IOrderQueries orderQueries, IIdentityService identityService)
        {
            if (mediator == null)
            {
                throw new ArgumentNullException(nameof(mediator));
            }

            if (orderQueries == null)
            {
                throw new ArgumentNullException(nameof(orderQueries));
            }

            if (identityService == null)
            {
                throw new ArgumentException(nameof(identityService));
            }

            _mediator = mediator;
            _orderQueries = orderQueries;
            _identityService = identityService;
        }

        [Route("new")]
        [HttpPost]
        public async Task<IActionResult> AddOrder([FromBody]NewOrderRequest order)
        {
            if (order.CardTypeId == 0)
                order.CardTypeId = 1;

            order.Buyer = _identityService.GetUserIdentity();

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
    }
}


