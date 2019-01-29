using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebhookClient.Models;

namespace WebhookClient.Controllers
{
    [ApiController]
    [Route("webhook-received")]
    public class WebhooksReceivedController : Controller
    {

        private readonly Settings _settings;
        
        public WebhooksReceivedController(IOptions<Settings> settings)
        {
            _settings = settings.Value;
        }

        public IActionResult NewWebhook(WebhookData hook)
        {
            var header = Request.Headers[HeaderNames.WebHookCheckHeader];
            var token = header.FirstOrDefault();
            if (!_settings.ValidateToken || _settings.Token == token)
            {
                var received = HttpContext.Session.Get<IEnumerable<WebHookReceived>>(SessionKeys.HooksKey)?.ToList() ?? new List<WebHookReceived>();
                received.Add(new WebHookReceived()
                {
                    Data = hook.Payload,
                    When = hook.When,
                    Token = token
                });
                return Ok();
            }

            return BadRequest();
        }
    }
}
