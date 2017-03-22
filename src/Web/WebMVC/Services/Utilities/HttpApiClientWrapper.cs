using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using Polly.Wrap;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebMVC.Services.Utilities
{
    /// <summary>
    /// HttpClient wrapper that integrates Retry and Circuit
    /// breaker policies when calling to Api services. 
    /// Currently is ONLY implemented for the ASP MVC
    /// and Xamarin App
    /// </summary>
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
                5,
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
                5,
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

        public Task<string> GetStringAsync(string uri) =>
            HttpInvoker(() => 
                _client.GetStringAsync(uri));

        public Task<HttpResponseMessage> PostAsync<T>(string uri, T item) =>
            // a new StringContent must be created for each retry 
            // as it is disposed after each call
            HttpInvoker(() =>
                {
                    var response = _client.PostAsync(uri, new StringContent(JsonConvert.SerializeObject(item), System.Text.Encoding.UTF8, "application/json"));
                    // raise exception if not success response
                    // needed for circuit breaker to track fails
                    response.Result.EnsureSuccessStatusCode();
                    return response;
                });

        public Task<HttpResponseMessage> DeleteAsync(string uri) =>
            HttpInvoker(() => _client.DeleteAsync(uri));


        private Task<T> HttpInvoker<T>(Func<Task<T>> action) =>
            // Executes the action applying all 
            // the policies defined in the wrapper
            _policyWrapper.ExecuteAsync(() => action());
    }

}
