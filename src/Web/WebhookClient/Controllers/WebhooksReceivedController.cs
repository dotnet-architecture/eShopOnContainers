using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebhookClient.Models;
using WebhookClient.Services;

namespace WebhookClient.Controllers
{
    [ApiController]
    [Route("webhook-received")]
    public class WebhooksReceivedController : Controller
    {

        private readonly Settings _settings;
        private readonly ILogger _logger;
        private readonly IHooksRepository _hooksRepository;
        
        public WebhooksReceivedController(IOptions<Settings> settings, ILogger<WebhooksReceivedController> logger, IHooksRepository hooksRepository)
        {
            _settings = settings.Value;
            _logger = logger;
            _hooksRepository = hooksRepository;
        }

        [HttpPost]
        public async Task<IActionResult> NewWebhook(WebhookData hook)
        {
            var header = Request.Headers[HeaderNames.WebHookCheckHeader];
            var token = header.FirstOrDefault();

            _logger.LogInformation($"Received hook with token {token}. My token is {_settings.Token}. Token validation is set to {_settings.ValidateToken}");

            if (!_settings.ValidateToken || _settings.Token == token)
            {
                _logger.LogInformation($"Received hook is going to be processed");
                var newHook = new WebHookReceived()
                {
                    Data = hook.Payload,
                    When = hook.When,
                    Token = token
                };
                await _hooksRepository.AddNew(newHook);
                _logger.LogInformation($"Received hook was processed.");
                return Ok(newHook);
            }

            _logger.LogInformation($"Received hook is NOT processed - Bad Request returned.");
            return BadRequest();
        }
    }
}
