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

        // Notice that these (and other methods below) are Task
        // returning asynchronous methods. But, they do not
        // have the 'async' modifier, and do not contain
        // any 'await statements. In each of these methods,
        // the only asynchronous call is the last (or only)
        // statement of the method. In those instances,
        // a Task returning method that does not use the 
        // async modifier is preferred. The compiler generates
        // synchronous code for this method, but returns the 
        // task from the underlying asynchronous method. The
        // generated code does not contain the state machine
        // generated for asynchronous methods.
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

