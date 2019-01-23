using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using WebhookClient.Models;

namespace WebhookClient.Pages
{

    public class RegisterWebhookModel : PageModel
    {
        private readonly Settings _settings;
        private readonly IHttpClientFactory _httpClientFactory;

        [BindProperty] public string Token { get; set; }


        public RegisterWebhookModel(IOptions<Settings> settings, IHttpClientFactory httpClientFactory)
        {
            _settings = settings.Value;
            _httpClientFactory = httpClientFactory;
        }

        public void OnGet()
        {
            Token = _settings.Token;
        }

        public async Task OnPost()
        {
            var protocol = Request.IsHttps ? "https" : "http";
            var selfurl = !string.IsNullOrEmpty(_settings.SelfUrl) ? _settings.SelfUrl : $"{protocol}://{Request.Host}/{Request.PathBase}";
            var granturl = $"{selfurl}check";
            var url = $"{selfurl}webhook";
            var client = _httpClientFactory.CreateClient("GrantClient");

            var payload = new WebhookSubscriptionRequest()
            {
                Event = "OrderShipped",
                GrantUrl = granturl,
                Url = url,
                Token = Token
            };
            var response = await client.PostAsync<WebhookSubscriptionRequest>(_settings.WebhooksUrl + "/api/v1/webhooks", payload, new JsonMediaTypeFormatter());

            RedirectToPage("Index");
        }
    }
}