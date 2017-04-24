using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.eShopOnContainers.Services.Basket.API.Model;
using Microsoft.AspNetCore.Authorization;

namespace Microsoft.eShopOnContainers.Services.Basket.API.Controllers
{
    //TODO NOTE: Right now this is a very chunky API, as the app evolves it is possible we would
    //want to make the actions more fine grained, add basket item as an action for example.
    //If this is the case we should also investigate changing the serialization format used for Redis,
    //using a HashSet instead of a simple string.
    [Route("/")]
    [Authorize]
    public class BasketController : Controller
    {
        private IBasketRepository _repository;

        public BasketController(IBasketRepository repository)
        {
            _repository = repository;
        }
        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var basket = await _repository.GetBasketAsync(id);

            return Ok(basket);
        }

        // POST api/values
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]CustomerBasket value)
        {
            var basket = await _repository.UpdateBasketAsync(value);

            return Ok(basket);
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(string id)
        {
            _repository.DeleteBasketAsync(id);
        }
    }
}
