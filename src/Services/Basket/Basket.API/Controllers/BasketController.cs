using Basket.API.IntegrationEvents.Events;
using Basket.API.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
using Microsoft.eShopOnContainers.Services.Basket.API.Model;
using Microsoft.eShopOnContainers.Services.Basket.API.Services;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Microsoft.eShopOnContainers.Services.Basket.API.Controllers
{
    [Route("api/v1/[controller]")]
    [Authorize]
    public class BasketController : Controller
    {
        private readonly IBasketRepository _repository;
        private readonly IIdentityService _identitySvc;
        private readonly IEventBus _eventBus;

        public BasketController(IBasketRepository repository,
            IIdentityService identityService,
            IEventBus eventBus)
        {
            _repository = repository;
            _identitySvc = identityService;
            _eventBus = eventBus;
        }

        // GET /id
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CustomerBasket), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Get(string id)
        {
            var basket = await _repository.GetBasketAsync(id);
            if (basket == null)
            {
                return NotFound();
            }

            return Ok(basket);
        }

        // POST /value
        [HttpPost]
        [ProducesResponseType(typeof(CustomerBasket), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Post([FromBody]CustomerBasket value)
        {
            var basket = await _repository.UpdateBasketAsync(value);

            return Ok(basket);
        }

        [Route("checkout")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Checkout([FromBody]BasketCheckout basketCheckout, [FromHeader(Name = "x-requestid")] string requestId)
        {
            var userId = _identitySvc.GetUserIdentity();
            var userName = User.FindFirst(x => x.Type == "unique_name").Value;

            basketCheckout.RequestId = (Guid.TryParse(requestId, out Guid guid) && guid != Guid.Empty) ?
                guid : basketCheckout.RequestId;

            var basket = await _repository.GetBasketAsync(userId);

            if (basket == null)
            {
                return BadRequest();
            }

            var eventMessage = new UserCheckoutAcceptedIntegrationEvent(userId, userName, basketCheckout.City, basketCheckout.Street,
                basketCheckout.State, basketCheckout.Country, basketCheckout.ZipCode, basketCheckout.CardNumber, basketCheckout.CardHolderName,
                basketCheckout.CardExpiration, basketCheckout.CardSecurityNumber, basketCheckout.CardTypeId, basketCheckout.Buyer, basketCheckout.RequestId, basket);

            // Once basket is checkout, sends an integration event to
            // ordering.api to convert basket to order and proceeds with
            // order creation process
            _eventBus.Publish(eventMessage);            

            return Accepted();
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(string id)
        {
            _repository.DeleteBasketAsync(id);
        }

    }
}
