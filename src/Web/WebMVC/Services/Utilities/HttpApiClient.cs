using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebMVC.Services.Utilities
{
    public class HttpApiClient : IHttpClient
    {
        private HttpClient _client;
        private ILogger _logger;
        public HttpClient Inst => _client;
        public HttpApiClient()
        {
            _client = new HttpClient();
            _logger = new LoggerFactory().CreateLogger(nameof(HttpApiClientWrapper));
        }

        public async Task<string> GetStringAsync(string uri)
        {
            return await HttpInvoker(async () =>
                await _client.GetStringAsync(uri));
        }

        public async Task<HttpResponseMessage> PostAsync<T>(string uri, T item)
        {
            var contentString = new StringContent(JsonConvert.SerializeObject(item), System.Text.Encoding.UTF8, "application/json");
            return await HttpInvoker(async () =>
                    await _client.PostAsync(uri, contentString));
        }

        public async Task<HttpResponseMessage> DeleteAsync(string uri)
        {
            return await HttpInvoker(async () =>
                await _client.DeleteAsync(uri));
        }

        private async Task<T> HttpInvoker<T>(Func<Task<T>> action)
        {
            return await action();
        }
    }
}

