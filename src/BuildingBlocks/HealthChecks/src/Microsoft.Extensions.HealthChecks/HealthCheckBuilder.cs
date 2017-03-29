// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Microsoft.Extensions.HealthChecks
{
    public class HealthCheckBuilder
    {
        private readonly Dictionary<string, IHealthCheck> _checks;

        public HealthCheckBuilder()
        {
            _checks = new Dictionary<string, IHealthCheck>(StringComparer.OrdinalIgnoreCase);
            DefaultCacheDuration = TimeSpan.FromMinutes(5);
        }

        public IReadOnlyDictionary<string, IHealthCheck> Checks => _checks;

        public TimeSpan DefaultCacheDuration { get; private set; }

        public HealthCheckBuilder AddCheck(string name, IHealthCheck check)
        {
            Guard.ArgumentNotNullOrWhitespace(nameof(name), name);
            Guard.ArgumentNotNull(nameof(check), check);

            _checks.Add(name, check);
            return this;
        }

        public HealthCheckBuilder WithDefaultCacheDuration(TimeSpan duration)
        {
            Guard.ArgumentValid(duration >= TimeSpan.Zero, nameof(duration), "Duration must be zero (disabled) or a positive duration");

            DefaultCacheDuration = duration;
            return this;
        }
    }
}
