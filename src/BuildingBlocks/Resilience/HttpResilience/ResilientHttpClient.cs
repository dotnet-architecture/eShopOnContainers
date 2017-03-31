using Microsoft.eShopOnContainers.BuildingBlocks.Resilience.HttpResilience.Policies;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using Polly.Wrap;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.eShopOnContainers.BuildingBlocks.Resilience.HttpResilience
{
    /// <summary>
    /// HttpClient wrapper that integrates Retry and Circuit
    /// breaker policies when invoking HTTP services. 
    /// </summary>
    public class ResilientHttpClient : IHttpClient
    {
        private HttpClient _client;
        private PolicyWrap _policyWrapper;
        private ILogger<ResilientHttpClient> _logger;
        public HttpClient Inst => _client;

        public ResilientHttpClient(List<Policy> policies, ILogger<ResilientHttpClient> logger)
        {
            _client = new HttpClient();
            _logger = logger;

            // Add Policies to be applied
            _policyWrapper = Policy.WrapAsync(policies.ToArray());
        }

        public ResilientHttpClient(List<ResilientPolicy> policies, ILogger<ResilientHttpClient> logger)
        {
            _client = new HttpClient();
            _logger = logger;

            // Add Policies to be applied
            _policyWrapper = Policy.WrapAsync(GeneratePolicies(policies));
        }

        private Policy[] GeneratePolicies(IList<ResilientPolicy> policies)
        {
            var pollyPolicies = new List<Policy>();

            foreach (var policy in policies)
            {
                switch (policy)
                {
                    case RetryPolicy retryPolicy:
                        pollyPolicies.Add(
                            CreateRetryPolicy(
                                retryPolicy.Retries,
                                retryPolicy.BackoffSeconds,
                                retryPolicy.ExponentialBackoff));
                        break;

                    case CircuitBreakerPolicy circuitPolicy:
                        pollyPolicies.Add(
                            CreateCircuitBreakerPolicy(
                                circuitPolicy.ExceptionsAllowedBeforeBreaking,
                                circuitPolicy.DurationOfBreakInMinutes));
                        break;
                }
            }

            return pollyPolicies.ToArray();
        }

        private Policy CreateRetryPolicy(int retries, int backoffSeconds, bool exponentialBackoff) =>            
            Policy.Handle<HttpRequestException>()
                .WaitAndRetryAsync(
                retries,               
                retryAttempt => exponentialBackoff ? TimeSpan.FromSeconds(Math.Pow(backoffSeconds, retryAttempt)) : TimeSpan.FromSeconds(backoffSeconds),
                (exception, timeSpan, retryCount, context) =>
                {
                    var msg = $"Retry {retryCount} implemented with Polly's RetryPolicy " +
                        $"of {context.PolicyKey} " +
                        $"at {context.ExecutionKey}, " +
                        $"due to: {exception}.";
                    _logger.LogWarning(msg);
                    _logger.LogDebug(msg);
                }
            );

        private Policy CreateCircuitBreakerPolicy(int exceptionsAllowedBeforeBreaking, int durationOfBreakInMinutes) =>
           Policy.Handle<HttpRequestException>()
           .CircuitBreakerAsync(
               exceptionsAllowedBeforeBreaking,
               TimeSpan.FromMinutes(durationOfBreakInMinutes),
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

        public Task<string> GetStringAsync(string uri) =>
            HttpInvoker(() => 
                _client.GetStringAsync(uri));

        public Task<HttpResponseMessage> PostAsync<T>(string uri, T item) =>
            // a new StringContent must be created for each retry 
            // as it is disposed after each call
            HttpInvoker(() =>
            {
                var response = _client.PostAsync(uri, new StringContent(JsonConvert.SerializeObject(item), System.Text.Encoding.UTF8, "application/json"));
                // raise exception if HttpResponseCode 500 
                // needed for circuit breaker to track fails
                if (response.Result.StatusCode == HttpStatusCode.InternalServerError)
                {
                    throw new HttpRequestException();
                }

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
