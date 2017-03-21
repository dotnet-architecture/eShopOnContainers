using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using Polly.Wrap;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebMVC.Services.Utilities
{
    public class HttpApiClientWrapper : IHttpClient
    {
        private HttpClient _client;
        private PolicyWrap _policyWrapper;
        private ILogger _logger;
        public HttpClient Inst => _client;
        public HttpApiClientWrapper()
        {
            _client = new HttpClient();
            _logger = new LoggerFactory().CreateLogger(nameof(HttpApiClientWrapper));

            // Add Policies to be applied
            _policyWrapper = Policy.WrapAsync(
                CreateRetryPolicy(),
                CreateCircuitBreakerPolicy()
            );
        }

        private Policy CreateCircuitBreakerPolicy() =>
            Policy.Handle<HttpRequestException>()
            .CircuitBreakerAsync(
                // number of exceptions before breaking circuit
                3,
                // time circuit opened before retry
                TimeSpan.FromMinutes(1),
                (exception, duration) =>
                {
                    // on circuit opened
                    _logger.LogTrace("Circuit breaker opened");
                },
                () =>
                {
                    // on circuit closed
                    _logger.LogTrace("Circuit breaker reset");
                }
            );

        private Policy CreateRetryPolicy() =>
            Policy.Handle<HttpRequestException>()
            .WaitAndRetryAsync(
                // number of retries
                3, 
                // exponential backofff
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                // on retry
                (exception, timeSpan, retryCount, context) =>  
                {
                    _logger.LogTrace($"Retry {retryCount} " +
                        $"of {context.PolicyKey} " +
                        $"at {context.ExecutionKey}, " +
                        $"due to: {exception}.");
                }
            );

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
            HttpInvoker(() => _client.GetStringAsync(uri));

        public Task<HttpResponseMessage> PostAsync<T>(string uri, T item) =>
            // a new StringContent must be created for each retry 
            // as it is disposed after each call
            HttpInvoker(() =>_client.PostAsync(uri, 
                new StringContent(JsonConvert.SerializeObject(item), 
                System.Text.Encoding.UTF8, "application/json")));

        public Task<HttpResponseMessage> DeleteAsync(string uri) =>
            HttpInvoker(() => _client.DeleteAsync(uri));


        private Task<T> HttpInvoker<T>(Func<Task<T>> action) =>
            // Executes the action applying all 
            // the policies defined in the wrapper
            _policyWrapper.ExecuteAsync(() => action());
    }

}
