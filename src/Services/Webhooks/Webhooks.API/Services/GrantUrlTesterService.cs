using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Webhooks.API.Services
{
    class GrantUrlTesterService : IGrantUrlTesterService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger _logger;
        public GrantUrlTesterService(IHttpClientFactory factory, ILogger<IGrantUrlTesterService> logger)
        {
            _clientFactory = factory;
            _logger = logger;
        }

        public async Task<bool> TestGrantUrl(string urlHook, string url, string token)
        {
            if (!CheckSameOrigin(urlHook, url))
            {
                _logger.LogWarning($"Url of the hook ({urlHook} and the grant url ({url} do not belong to same origin)");
                return false;
            }


            var client = _clientFactory.CreateClient("GrantClient");
            var msg = new HttpRequestMessage(HttpMethod.Options, url);
            msg.Headers.Add("X-eshop-whtoken", token);
            _logger.LogInformation($"Sending the OPTIONS message to {url} with token {token ?? string.Empty}");
            try
            {
                var response = await client.SendAsync(msg);
                var tokenReceived = response.Headers.TryGetValues("X-eshop-whtoken", out var tokenValues) ? tokenValues.FirstOrDefault() : null;
                var tokenExpected = string.IsNullOrWhiteSpace(token) ? null : token;
                _logger.LogInformation($"Response code is {response.StatusCode} for url {url} and token in header was {tokenReceived} (expected token was {tokenExpected})");
                return response.IsSuccessStatusCode && tokenReceived == tokenExpected;
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Exception {ex.GetType().Name} when sending OPTIONS request. Url can't be granted.");
                return false;
            }
        }

        private bool CheckSameOrigin(string urlHook, string url)
        {
            var firstUrl = new Uri(urlHook, UriKind.Absolute);
            var secondUrl = new Uri(url, UriKind.Absolute);

            return firstUrl.Scheme == secondUrl.Scheme &&
                firstUrl.Port == secondUrl.Port &&
                firstUrl.Host == firstUrl.Host;
        }
    }
}
