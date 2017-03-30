// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.HealthChecks
{
    public static class HealthCheckServiceExtensions
    {
        public static Task<CompositeHealthCheckResult> CheckHealthAsync(this IHealthCheckService service)
        {
            Guard.ArgumentNotNull(nameof(service), service);

            return service.CheckHealthAsync(CheckStatus.Unhealthy, CancellationToken.None);
        }

        public static Task<CompositeHealthCheckResult> CheckHealthAsync(this IHealthCheckService service, CheckStatus partiallyHealthyStatus)
        {
            Guard.ArgumentNotNull(nameof(service), service);

            return service.CheckHealthAsync(partiallyHealthyStatus, CancellationToken.None);
        }

        public static Task<CompositeHealthCheckResult> CheckHealthAsync(this IHealthCheckService service, CancellationToken cancellationToken)
        {
            Guard.ArgumentNotNull(nameof(service), service);

            return service.CheckHealthAsync(CheckStatus.Unhealthy, cancellationToken);
        }
        public static Task<CompositeHealthCheckResult> CheckHealthAsync(this IHealthCheckService service, CheckStatus partiallyHealthyStatus, CancellationToken cancellationToken)
        {
            Guard.ArgumentNotNull(nameof(service), service);

            return service.CheckHealthAsync(partiallyHealthyStatus, cancellationToken);
        }
    }
}
