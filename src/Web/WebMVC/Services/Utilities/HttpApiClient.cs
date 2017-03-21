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
        
        public Task<string> GetStringAsync(string uri) =>
            _client.GetStringAsync(uri);

        public Task<HttpResponseMessage> PostAsync<T>(string uri, T item)
        {
            var contentString = new StringContent(JsonConvert.SerializeObject(item), System.Text.Encoding.UTF8, "application/json");
            return _client.PostAsync(uri, contentString);
        }

        public Task<HttpResponseMessage> DeleteAsync(string uri) =>
            _client.DeleteAsync(uri);
    }
}

