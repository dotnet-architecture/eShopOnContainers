namespace Microsoft.eShopOnContainers.Services.Ordering.API.Controllers
{
    using Application.Commands;
    using Application.Queries;
    using AspNetCore.Authorization;
    using MediatR;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using System;
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
        public async Task<IActionResult> AddOrder([FromBody]NewOrderViewModel order)
        {
            if (order.CardExpiration == DateTime.MinValue)
                order.CardExpiration = DateTime.Now;

            var newOrderRequest = new NewOrderRequest()
            {
                Buyer = GetUserName(), 
                CardTypeId = 1, //TODO
                CardHolderName = order.CardHolderName,
                CardNumber = order.CardNumber,
                CardExpiration = order.CardExpiration,
                CardSecurityNumber = order.CardSecurityNumber,
                State = order.ShippingState,
                City = order.ShippingCity,
                Country = order.ShippingCountry,
                Street = order.ShippingStreet
            };

            foreach (var orderItem in order.Items)
            {
                newOrderRequest.AddOrderItem(new Domain.OrderItem() {
                    Discount = orderItem.Discount,
                    ProductId = orderItem.ProductId,
                    UnitPrice = orderItem.UnitPrice,
                    ProductName = orderItem.ProductName,
                    Units = orderItem.Units
                });
            }

            var added = await _mediator.SendAsync(newOrderRequest);

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
            var order = await _orderQueries.GetOrder(orderId);

            return Ok(order);
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


