using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Webhooks.API.Model;

namespace Webhooks.API.Services
{
    public class WebhooksSender : IWebhooksSender
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger _logger;
        public WebhooksSender(IHttpClientFactory httpClientFactory, ILogger<WebhooksSender> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task SendAll(IEnumerable<WebhookSubscription> receivers, WebhookData data)
        {
            var client = _httpClientFactory.CreateClient();
            var json = JsonConvert.SerializeObject(data);
            var tasks = receivers.Select(r => OnSendData(r, json, client));
            await Task.WhenAll(tasks.ToArray());
        }

        private Task OnSendData(WebhookSubscription subs, string jsonData, HttpClient client)
        {
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(subs.DestUrl, UriKind.Absolute),
                Method = HttpMethod.Post,
                Content = new StringContent(jsonData, Encoding.UTF8, "application/json")
            };

            if (!string.IsNullOrWhiteSpace(subs.Token))
            {
                request.Headers.Add("X-eshop-whtoken", subs.Token);
            }
            _logger.LogDebug("Sending hook to {DestUrl} of type {Type}", subs.Type.ToString(), subs.Type.ToString());
            return client.SendAsync(request);
        }

    }
}
