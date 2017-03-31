using Microsoft.eShopOnContainers.BuildingBlocks.Resilience.HttpResilience.Policies;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.eShopOnContainers.BuildingBlocks.Resilience.HttpResilience
{
    public static class ResilientPolicyFactory
    {
        public static ResilientPolicy CreateRetryPolicy(int retries, int backoffSeconds, bool exponentialBackoff)
        {
            return new RetryPolicy(retries, backoffSeconds, exponentialBackoff); 
        }

        public static ResilientPolicy CreateCiscuitBreakerPolicy(int exceptionsAllowedBeforeBreaking, int durationOfBreakInMinutes)
        {
            return new CircuitBreakerPolicy(exceptionsAllowedBeforeBreaking, durationOfBreakInMinutes);
        }
    }
}
