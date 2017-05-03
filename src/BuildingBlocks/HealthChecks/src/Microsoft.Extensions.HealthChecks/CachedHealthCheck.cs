// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.HealthChecks
{
    public abstract class CachedHealthCheck
    {
        private static readonly TypeInfo HealthCheckTypeInfo = typeof(IHealthCheck).GetTypeInfo();

        private volatile int _writerCount;

        public CachedHealthCheck(string name, TimeSpan cacheDuration)
        {
            Guard.ArgumentNotNullOrEmpty(nameof(name), name);
            Guard.ArgumentValid(cacheDuration.TotalMilliseconds >= 0, nameof(cacheDuration), "Cache duration must be zero (disabled) or greater than zero.");

            Name = name;
            CacheDuration = cacheDuration;
        }

        public IHealthCheckResult CachedResult { get; internal set; }

        public TimeSpan CacheDuration { get; }

        public DateTimeOffset CacheExpiration { get; internal set; }

        public string Name { get; }

        protected virtual DateTimeOffset UtcNow => DateTimeOffset.UtcNow;

        protected abstract IHealthCheck Resolve(IServiceProvider serviceProvider);

        public async ValueTask<IHealthCheckResult> RunAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken = default(CancellationToken))
        {
            while (CacheExpiration <= UtcNow)
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
                    var check = Resolve(serviceProvider);
                    CachedResult = await check.CheckAsync(cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    CachedResult = HealthCheckResult.Unhealthy("The health check operation timed out");
                }
                catch (Exception ex)
                {
                    CachedResult = HealthCheckResult.Unhealthy($"Exception during check: {ex.GetType().FullName}");
                }

                CacheExpiration = UtcNow + CacheDuration;
                _writerCount = 0;
                break;
            }

            return CachedResult;
        }

        public static CachedHealthCheck FromHealthCheck(string name, TimeSpan cacheDuration, IHealthCheck healthCheck)
        {
            Guard.ArgumentNotNull(nameof(healthCheck), healthCheck);

            return new TypeOrHealthCheck_HealthCheck(name, cacheDuration, healthCheck);
        }

        public static CachedHealthCheck FromType(string name, TimeSpan cacheDuration, Type healthCheckType)
        {
            Guard.ArgumentNotNull(nameof(healthCheckType), healthCheckType);
            Guard.ArgumentValid(HealthCheckTypeInfo.IsAssignableFrom(healthCheckType.GetTypeInfo()), nameof(healthCheckType), $"Health check must implement '{typeof(IHealthCheck).FullName}'.");

            return new TypeOrHealthCheck_Type(name, cacheDuration, healthCheckType);
        }

        class TypeOrHealthCheck_HealthCheck : CachedHealthCheck
        {
            private readonly IHealthCheck _healthCheck;

            public TypeOrHealthCheck_HealthCheck(string name, TimeSpan cacheDuration, IHealthCheck healthCheck) : base(name, cacheDuration)
                => _healthCheck = healthCheck;

            protected override IHealthCheck Resolve(IServiceProvider serviceProvider) => _healthCheck;
        }

        class TypeOrHealthCheck_Type : CachedHealthCheck
        {
            private readonly Type _healthCheckType;

            public TypeOrHealthCheck_Type(string name, TimeSpan cacheDuration, Type healthCheckType) : base(name, cacheDuration)
                => _healthCheckType = healthCheckType;

            protected override IHealthCheck Resolve(IServiceProvider serviceProvider)
                => (IHealthCheck)serviceProvider.GetRequiredService(_healthCheckType);
        }
    }
}
