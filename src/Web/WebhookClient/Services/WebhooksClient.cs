using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using WebhookClient.Models;

namespace WebhookClient.Services
{
    public class WebhooksClient : IWebhooksClient
    {

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly Settings _settings;
        public WebhooksClient(IHttpClientFactory httpClientFactory, IOptions<Settings> settings)
        {
            _httpClientFactory = httpClientFactory;
            _settings = settings.Value;
        }
        public async Task<IEnumerable<WebhookResponse>> LoadWebhooks()
        {
            var client = _httpClientFactory.CreateClient("GrantClient");
            var response = await client.GetAsync(_settings.WebhooksUrl + "/api/v1/webhooks");
            var json = await response.Content.ReadAsStringAsync();
            var subscriptions = JsonConvert.DeserializeObject<IEnumerable<WebhookResponse>>(json);
            return subscriptions;
        }
    }
}
