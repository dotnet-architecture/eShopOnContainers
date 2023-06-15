namespace Webhooks.API.Controllers;

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
    [ProducesResponseType(typeof(IEnumerable<WebhookSubscription>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListByUser()
    {
        var userId = _identityService.GetUserIdentity();
        var data = await _dbContext.Subscriptions.Where(s => s.UserId == userId).ToListAsync();
        return Ok(data);
    }

    [Authorize]
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(WebhookSubscription), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status418ImATeapot)]
    public async Task<IActionResult> SubscribeWebhook(WebhookSubscriptionRequest request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

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
            return StatusCode(StatusCodes.Status418ImATeapot, "Grant URL invalid");
        }
    }

    [Authorize]
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
