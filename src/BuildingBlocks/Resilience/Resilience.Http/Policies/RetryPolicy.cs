using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.eShopOnContainers.BuildingBlocks.Resilience.Http.Policies
{
    internal class RetryPolicy : ResiliencePolicy
    {
        public RetryPolicy(int retries, int backoffSeconds, bool exponentialBackoff)
        {
            Retries = retries;
            BackoffSeconds = backoffSeconds;
            ExponentialBackoff = exponentialBackoff;
        }

        public int Retries { get; }
        public int BackoffSeconds { get; }
        public bool ExponentialBackoff { get; }
    }
}
