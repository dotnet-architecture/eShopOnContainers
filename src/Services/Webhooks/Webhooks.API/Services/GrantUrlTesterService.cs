using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Webhooks.API.Services
{
    class GrantUrlTesterService : IGrantUrlTesterService
    {
        private readonly IHttpClientFactory _clientFactory;
        public GrantUrlTesterService(IHttpClientFactory factory)
        {
            _clientFactory = factory;
        }

        public async Task<bool> TestGrantUrl(string url, string token)
        {
            var client = _clientFactory.CreateClient("GrantClient");

            var msg = new HttpRequestMessage(HttpMethod.Options, url);
            msg.Headers.Add("X-eshop-whtoken", token);
            var response = await client.SendAsync(msg);
            return response.IsSuccessStatusCode;
        }
    }
}
