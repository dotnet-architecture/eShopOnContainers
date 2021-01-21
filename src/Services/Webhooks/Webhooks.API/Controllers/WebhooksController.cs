using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Webhooks.API.Infrastructure;
using Webhooks.API.Model;
using Webhooks.API.Services;

namespace Webhooks.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class WebhooksController : ControllerBase
    {
        private readonly WebhooksContext _dbContext;
        private readonly IIdentityService _identityService;
        private readonly IGrantUrlTesterService _grantUrlTester;

        public WebhooksController(WebhooksContext dbContext, IIdentityService identityService, IGrantUrlTesterService grantUrlTester)
        {
            _dbContext = dbContext;
            _identityService = identityService;
            _grantUrlTester = grantUrlTester;
        }

        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<WebhookSubscription>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ListByUser()
        {
            var userId = _identityService.GetUserIdentity();
            var data = await _dbContext.Subscriptions.Where(s => s.UserId == userId).ToListAsync();
            return Ok(data);
        }

        [Authorize]
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(WebhookSubscription), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetByUserAndId(int id)
        {
            var userId = _identityService.GetUserIdentity();
            var subscription = await _dbContext.Subscriptions.SingleOrDefaultAsync(s => s.Id == id && s.UserId == userId);
            if (subscription != null)
            {
                return Ok(subscription);
            }
            return NotFound($"Subscriptions {id} not found");
        }

        [Authorize]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(418)]
        public async Task<IActionResult> SubscribeWebhook(WebhookSubscriptionRequest request)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var userId = _identityService.GetUserIdentity();


            var grantOk = await _grantUrlTester.TestGrantUrl(request.Url, request.GrantUrl, request.Token ?? string.Empty);

            if (grantOk)
            {
                var subscription = new WebhookSubscription()
                {
                    Date = DateTime.UtcNow,
                    DestUrl = request.Url,
                    Token = request.Token,
                    Type = Enum.Parse<WebhookType>(request.Event, ignoreCase: true),
                    UserId = _identityService.GetUserIdentity()
                };

                _dbContext.Add(subscription);
                await _dbContext.SaveChangesAsync();
                return CreatedAtAction("GetByUserAndId", new { id = subscription.Id }, subscription);
            }
            else
            {
                return StatusCode(418, "Grant url can't be validated");
            }   
        }

        [Authorize]
        [HttpDelete("{id:int}")]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> UnsubscribeWebhook(int id)
        {
            var userId = _identityService.GetUserIdentity();
            var subscription = await _dbContext.Subscriptions.SingleOrDefaultAsync(s => s.Id == id && s.UserId == userId);

            if (subscription != null)
            {
                _dbContext.Remove(subscription);
                await _dbContext.SaveChangesAsync();
                return Accepted();
            }

            return NotFound($"Subscriptions {id} not found");
        }

    }


}
