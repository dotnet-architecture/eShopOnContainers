using Microsoft.eShopOnContainers.BuildingBlocks.Resilience.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Polly;
using System.Net.Http;

namespace Microsoft.eShopOnContainers.WebMVC.Infrastructure
{
    public class ResilientHttpClientFactory : IResilientHttpClientFactory
    {
        private readonly ILogger<ResilientHttpClient> _logger;

        public ResilientHttpClientFactory(ILogger<ResilientHttpClient> logger) 
            =>_logger = logger;        

        public  ResilientHttpClient CreateResilientHttpClient()        
            => new ResilientHttpClient(CreatePolicies(), _logger);


        private Policy[] CreatePolicies()
            => new Policy[]
            {
                Policy.Handle<HttpRequestException>()                
                .WaitAndRetryAsync(
                    // number of retries
                    6,
                    // exponential backofff
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    // on retry
                    (exception, timeSpan, retryCount, context) =>
                    {
                        var msg = $"Retry {retryCount} implemented with Polly's RetryPolicy " +
                            $"of {context.PolicyKey} " +
                            $"at {context.ExecutionKey}, " +
                            $"due to: {exception}.";
                        _logger.LogWarning(msg);
                        _logger.LogDebug(msg);
                    }),
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
                   })};
    }
}
