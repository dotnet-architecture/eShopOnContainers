using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using WebhookClient.Models;

namespace WebhookClient.Pages
{

    public class IndexModel : PageModel
    {

        public IEnumerable<WebHookReceived> WebHooksReceived { get; private set; }

        public void OnGet()
        {
            WebHooksReceived = HttpContext.Session.Get<IEnumerable<WebHookReceived>>("webhooks.received") ?? Enumerable.Empty<WebHookReceived>();
        }
    }
}
