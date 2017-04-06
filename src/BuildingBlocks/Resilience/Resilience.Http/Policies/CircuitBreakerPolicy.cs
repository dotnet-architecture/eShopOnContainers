using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.eShopOnContainers.BuildingBlocks.Resilience.Http.Policies
{
    internal class CircuitBreakerPolicy : ResiliencePolicy
    {
        public CircuitBreakerPolicy(int exceptionsAllowedBeforeBreaking, int durationOfBreakInMinutes)
        {
            ExceptionsAllowedBeforeBreaking = exceptionsAllowedBeforeBreaking;
            DurationOfBreakInMinutes = durationOfBreakInMinutes;
        }

        public int ExceptionsAllowedBeforeBreaking { get; }
        public int DurationOfBreakInMinutes { get; }
    }
}
