using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using System;
using System.Net.Http;

namespace WebMVC.Infrastructure
{
    public class HttpClientDefaultPolicies
    {
        //Config for Retries with exponential backoff policy
        const int MAX_RETRIES = 6;
        const int SECONDS_BASE_FOR_EXPONENTIAL_BACKOFF = 2;

        //Config for Circuit Breaker policy
        const int EXCEPTIONS_ALLOWED_BEFORE_CIRCUIT_BREAKES = 5;
        const int DURATION_OF_BREAK_IN_MINUTES = 1;

        private readonly ILogger<HttpClientDefaultPolicies> _logger;

        public HttpClientDefaultPolicies(ILogger<HttpClientDefaultPolicies> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IAsyncPolicy<HttpResponseMessage> GetWaitAndRetryPolicy()
        {
            RetryPolicy retryPolicy = 
                    Policy.Handle<HttpRequestException>()
                        .WaitAndRetryAsync(
                            // Maximum number of retries
                            MAX_RETRIES,
                            // exponential backofff (2sg, 4sg, 8sg, 16sg, 32sg. etc.)
                            retryAttempt => TimeSpan.FromSeconds(Math.Pow(SECONDS_BASE_FOR_EXPONENTIAL_BACKOFF, retryAttempt)),
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

            return retryPolicy.AsAsyncPolicy<HttpResponseMessage>();
        }

        public IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
        {
            CircuitBreakerPolicy circuitBreakerPolicy =
                    Policy.Handle<HttpRequestException>()
                        .CircuitBreakerAsync(
                           // Number of exceptions before breaking the circuit
                           EXCEPTIONS_ALLOWED_BEFORE_CIRCUIT_BREAKES,
                           // Duration of break
                           TimeSpan.FromMinutes(DURATION_OF_BREAK_IN_MINUTES),
                           (exception, duration) =>
                           {
                               // On circuit opened (Circuit is broken)
                               _logger.LogTrace("Circuit has been broken");
                           },
                           () =>
                           {
                               // on circuit closed
                               _logger.LogTrace("Circuit has been reset");
                           });

            
            return circuitBreakerPolicy.AsAsyncPolicy<HttpResponseMessage>();
        }
    }
}
