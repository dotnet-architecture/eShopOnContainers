namespace WebhookClient.Controllers;

[ApiController]
[Route("webhook-received")]
public class WebhooksReceivedController : Controller
{
    private readonly WebhookClientOptions _options;
    private readonly ILogger _logger;
    private readonly IHooksRepository _hooksRepository;

    public WebhooksReceivedController(IOptions<WebhookClientOptions> options, ILogger<WebhooksReceivedController> logger, IHooksRepository hooksRepository)
    {
        _options = options.Value;
        _logger = logger;
        _hooksRepository = hooksRepository;
    }

    [HttpPost]
    public async Task<IActionResult> NewWebhook(WebhookData hook)
    {
        string token = Request.Headers[HeaderNames.WebHookCheckHeader];

        _logger.LogInformation("Received hook with token {Token}. My token is {MyToken}. Token validation is set to {ValidateToken}", token, _options.Token, _options.ValidateToken);

        if (!_options.ValidateToken || _options.Token == token)
        {
            _logger.LogInformation("Received hook is going to be processed");
            var newHook = new WebHookReceived()
            {
                Data = hook.Payload,
                When = hook.When,
                Token = token
            };
            await _hooksRepository.AddNew(newHook);
            _logger.LogInformation("Received hook was processed.");
            return Ok(newHook);
        }

        _logger.LogInformation("Received hook is NOT processed - Bad Request returned.");
        return BadRequest();
    }
}
