using Microsoft.Extensions.Logging;
using Polly;
using System;
using System.Net.Http;

namespace WebMVC.Infrastructure
{
    public class HttpClientDefaultPolicies
    {
        const int RETRY_COUNT = 6;
        const int EXCEPTIONS_ALLOWED_BEFORE_CIRCUIT_BREAKER = 5;

        private readonly ILogger<HttpClientDefaultPolicies> _logger;

        public HttpClientDefaultPolicies(ILogger<HttpClientDefaultPolicies> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Policy GetWaitAndRetryPolicy()
        {
            return Policy.Handle<HttpRequestException>()
                .WaitAndRetryAsync(
                    // number of retries
                    RETRY_COUNT,
                    // exponential backofff
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    // on retry
                    (exception, timeSpan, retryCount, context) =>
                    {
                        var msg = $"Retry {retryCount} implemented with Polly's RetryPolicy " +
                            $"of {context.PolicyKey} " +
                            $"at {context.OperationKey}, " +
                            $"due to: {exception}.";
                        _logger.LogWarning(msg);
                        _logger.LogDebug(msg);
                    });
        }

        public Policy GetCircuitBreakerPolicy()
        {
            return Policy.Handle<HttpRequestException>()
                .CircuitBreakerAsync(
                   // number of exceptions before breaking circuit
                   EXCEPTIONS_ALLOWED_BEFORE_CIRCUIT_BREAKER,
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
                   });
        }
    }
}
