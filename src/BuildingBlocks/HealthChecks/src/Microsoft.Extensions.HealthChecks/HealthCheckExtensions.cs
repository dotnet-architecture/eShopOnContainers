// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.HealthChecks
{
    public static class HealthCheckExtensions
    {
        public static ValueTask<IHealthCheckResult> CheckAsync(this IHealthCheck healthCheck)
        {
            Guard.ArgumentNotNull(nameof(healthCheck), healthCheck);

            return healthCheck.CheckAsync(CancellationToken.None);
        }
    }
}
