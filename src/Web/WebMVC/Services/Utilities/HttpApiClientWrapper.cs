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

        private Policy CreateCircuitBreakerPolicy()
        {
            return Policy
                .Handle<HttpRequestException>()
                .CircuitBreakerAsync(
                    // number of exceptions before breaking circuit
                    5, 
                    // time circuit opened before retry
                    TimeSpan.FromMinutes(1), 
                    (exception, duration) => {
                        // on circuit opened
                        _logger.LogTrace("Circuit breaker opened");
                    }, 
                    () => {
                        // on circuit closed
                        _logger.LogTrace("Circuit breaker reset");
                    }
                );
        }

        private Policy CreateRetryPolicy()
        {
            return Policy
                .Handle<HttpRequestException>()
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
                 });
        }

        public async Task<string> GetStringAsync(string uri)
        {
            return await HttpInvoker(async () => 
                await _client.GetStringAsync(uri));
        }

        public async Task<HttpResponseMessage> PostAsync<T>(string uri, T item)
        {
            // a new StringContent must be created for each retry 
            // as it is disposed after each call
            return await HttpInvoker(async () =>
                    await _client.PostAsync(uri, 
                        new StringContent(JsonConvert.SerializeObject(item), 
                        System.Text.Encoding.UTF8, "application/json")));
        }

        public async Task<HttpResponseMessage> DeleteAsync(string uri)
        {
            return await HttpInvoker(async () =>
                await _client.DeleteAsync(uri));
        }

        private async Task<T> HttpInvoker<T>(Func<Task<T>> action)
        {
            // Executes the action applying all 
            // the policies defined in the wrapper
            return await _policyWrapper
                .ExecuteAsync(async () => await action());
        }
    }

}
