// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.HealthChecks
{
    public static partial class HealthCheckBuilderExtensions
    {
        // Lambda versions of AddCheck for Func/Func<Task>/Func<ValueTask>

        public static HealthCheckBuilder AddCheck(this HealthCheckBuilder builder, string name, Func<IHealthCheckResult> check)
        {
            Guard.ArgumentNotNull(nameof(builder), builder);

            builder.AddCheck(name, HealthCheck.FromCheck(check, builder.DefaultCacheDuration));
            return builder;
        }

        public static HealthCheckBuilder AddCheck(this HealthCheckBuilder builder, string name, Func<CancellationToken, IHealthCheckResult> check)
        {
            Guard.ArgumentNotNull(nameof(builder), builder);

            builder.AddCheck(name, HealthCheck.FromCheck(check, builder.DefaultCacheDuration));
            return builder;
        }

        public static HealthCheckBuilder AddCheck(this HealthCheckBuilder builder, string name, Func<IHealthCheckResult> check, TimeSpan cacheDuration)
        {
            Guard.ArgumentNotNull(nameof(builder), builder);

            builder.AddCheck(name, HealthCheck.FromCheck(check, cacheDuration));
            return builder;
        }

        public static HealthCheckBuilder AddCheck(this HealthCheckBuilder builder, string name, Func<CancellationToken, IHealthCheckResult> check, TimeSpan cacheDuration)
        {
            Guard.ArgumentNotNull(nameof(builder), builder);

            builder.AddCheck(name, HealthCheck.FromCheck(check, cacheDuration));
            return builder;
        }

        public static HealthCheckBuilder AddCheck(this HealthCheckBuilder builder, string name, Func<Task<IHealthCheckResult>> check)
        {
            Guard.ArgumentNotNull(nameof(builder), builder);

            builder.AddCheck(name, HealthCheck.FromTaskCheck(check, builder.DefaultCacheDuration));
            return builder;
        }

        public static HealthCheckBuilder AddCheck(this HealthCheckBuilder builder, string name, Func<CancellationToken, Task<IHealthCheckResult>> check)
        {
            Guard.ArgumentNotNull(nameof(builder), builder);

            builder.AddCheck(name, HealthCheck.FromTaskCheck(check, builder.DefaultCacheDuration));
            return builder;
        }

        public static HealthCheckBuilder AddCheck(this HealthCheckBuilder builder, string name, Func<Task<IHealthCheckResult>> check, TimeSpan cacheDuration)
        {
            Guard.ArgumentNotNull(nameof(builder), builder);

            builder.AddCheck(name, HealthCheck.FromTaskCheck(check, cacheDuration));
            return builder;
        }

        public static HealthCheckBuilder AddCheck(this HealthCheckBuilder builder, string name, Func<CancellationToken, Task<IHealthCheckResult>> check, TimeSpan cacheDuration)
        {
            Guard.ArgumentNotNull(nameof(builder), builder);

            builder.AddCheck(name, HealthCheck.FromTaskCheck(check, cacheDuration));
            return builder;
        }

        public static HealthCheckBuilder AddValueTaskCheck(this HealthCheckBuilder builder, string name, Func<ValueTask<IHealthCheckResult>> check)
        {
            Guard.ArgumentNotNull(nameof(builder), builder);

            builder.AddCheck(name, HealthCheck.FromValueTaskCheck(check, builder.DefaultCacheDuration));
            return builder;
        }

        public static HealthCheckBuilder AddValueTaskCheck(this HealthCheckBuilder builder, string name, Func<CancellationToken, ValueTask<IHealthCheckResult>> check)
        {
            Guard.ArgumentNotNull(nameof(builder), builder);

            builder.AddCheck(name, HealthCheck.FromValueTaskCheck(check, builder.DefaultCacheDuration));
            return builder;
        }

        public static HealthCheckBuilder AddValueTaskCheck(this HealthCheckBuilder builder, string name, Func<ValueTask<IHealthCheckResult>> check, TimeSpan cacheDuration)
        {
            Guard.ArgumentNotNull(nameof(builder), builder);

            builder.AddCheck(name, HealthCheck.FromValueTaskCheck(check, cacheDuration));
            return builder;
        }

        public static HealthCheckBuilder AddValueTaskCheck(this HealthCheckBuilder builder, string name, Func<CancellationToken, ValueTask<IHealthCheckResult>> check, TimeSpan cacheDuration)
        {
            Guard.ArgumentNotNull(nameof(builder), builder);

            builder.AddCheck(name, HealthCheck.FromValueTaskCheck(check, cacheDuration));
            return builder;
        }
    }
}
