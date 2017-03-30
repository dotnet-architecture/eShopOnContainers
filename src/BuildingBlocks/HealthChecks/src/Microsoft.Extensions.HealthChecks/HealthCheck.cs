// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.HealthChecks
{
    public class HealthCheck : IHealthCheck
    {
        private DateTimeOffset _cacheExpiration;
        private IHealthCheckResult _cachedResult;
        private volatile int _writerCount;

        protected HealthCheck(Func<CancellationToken, ValueTask<IHealthCheckResult>> check, TimeSpan cacheDuration)
        {
            Guard.ArgumentNotNull(nameof(check), check);
            Guard.ArgumentValid(cacheDuration >= TimeSpan.Zero, nameof(cacheDuration), "Cache duration must either be zero (disabled) or a positive value");

            Check = check;
            CacheDuration = cacheDuration;
        }

        public TimeSpan CacheDuration { get; }

        protected Func<CancellationToken, ValueTask<IHealthCheckResult>> Check { get; }

        protected virtual DateTimeOffset UtcNow => DateTimeOffset.UtcNow;

        public async ValueTask<IHealthCheckResult> CheckAsync(CancellationToken cancellationToken)
        {
            while (_cacheExpiration <= UtcNow)
            {
                // Can't use a standard lock here because of async, so we'll use this flag to determine when we should write a value,
                // and the waiters who aren't allowed to write will just spin wait for the new value.
                if (Interlocked.Exchange(ref _writerCount, 1) != 0)
                {
                    await Task.Delay(5, cancellationToken).ConfigureAwait(false);
                    continue;
                }

                try
                {
                    _cachedResult = await Check(cancellationToken).ConfigureAwait(false);
                    _cacheExpiration = UtcNow + CacheDuration;
                    break;
                }
                finally
                {
                    _writerCount = 0;
                }
            }

            return _cachedResult;
        }

        public static HealthCheck FromCheck(Func<IHealthCheckResult> check, TimeSpan cacheDuration)
            => new HealthCheck(token => new ValueTask<IHealthCheckResult>(check()), cacheDuration);

        public static HealthCheck FromCheck(Func<CancellationToken, IHealthCheckResult> check, TimeSpan cacheDuration)
            => new HealthCheck(token => new ValueTask<IHealthCheckResult>(check(token)), cacheDuration);

        public static HealthCheck FromTaskCheck(Func<Task<IHealthCheckResult>> check, TimeSpan cacheDuration)
            => new HealthCheck(token => new ValueTask<IHealthCheckResult>(check()), cacheDuration);

        public static HealthCheck FromTaskCheck(Func<CancellationToken, Task<IHealthCheckResult>> check, TimeSpan cacheDuration)
            => new HealthCheck(token => new ValueTask<IHealthCheckResult>(check(token)), cacheDuration);

        public static HealthCheck FromValueTaskCheck(Func<ValueTask<IHealthCheckResult>> check, TimeSpan cacheDuration)
            => new HealthCheck(token => check(), cacheDuration);

        public static HealthCheck FromValueTaskCheck(Func<CancellationToken, ValueTask<IHealthCheckResult>> check, TimeSpan cacheDuration)
            => new HealthCheck(check, cacheDuration);
    }
}
