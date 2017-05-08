// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Microsoft.Extensions.HealthChecks
{
    public class HealthCheckBuilder
    {
        private readonly Dictionary<string, CachedHealthCheck> _checksByName;
        private readonly HealthCheckGroup _currentGroup;
        private readonly Dictionary<string, HealthCheckGroup> _groups;

        public HealthCheckBuilder()
        {
            _checksByName = new Dictionary<string, CachedHealthCheck>(StringComparer.OrdinalIgnoreCase);
            _currentGroup = new HealthCheckGroup(string.Empty, CheckStatus.Unhealthy);
            _groups = new Dictionary<string, HealthCheckGroup>(StringComparer.OrdinalIgnoreCase)
            {
                [string.Empty] = _currentGroup
            };

            DefaultCacheDuration = TimeSpan.FromMinutes(5);
        }

        /// <summary>
        /// This constructor should only be used when creating a grouped health check builder.
        /// </summary>
        public HealthCheckBuilder(HealthCheckBuilder rootBuilder, HealthCheckGroup currentGroup)
        {
            Guard.ArgumentNotNull(nameof(rootBuilder), rootBuilder);
            Guard.ArgumentNotNull(nameof(currentGroup), currentGroup);

            _checksByName = rootBuilder._checksByName;
            _currentGroup = currentGroup;
            _groups = rootBuilder._groups;

            DefaultCacheDuration = rootBuilder.DefaultCacheDuration;
        }

        /// <summary>
        /// Gets the registered checks, indexed by check name.
        /// </summary>
        public IReadOnlyDictionary<string, CachedHealthCheck> ChecksByName => _checksByName;

        /// <summary>
        /// Gets the current default cache duration used when registering checks.
        /// </summary>
        public TimeSpan DefaultCacheDuration { get; private set; }

        /// <summary>
        /// Gets the registered groups, indexed by group name. The root group's name is <see cref="string.Empty"/>.
        /// </summary>
        public IReadOnlyDictionary<string, HealthCheckGroup> Groups => _groups;

        /// <summary>
        /// Registers a health check type that will later be resolved via dependency
        /// injection.
        /// </summary>
        public HealthCheckBuilder AddCheck<TCheck>(string checkName, TimeSpan cacheDuration) where TCheck : class, IHealthCheck
        {
            Guard.ArgumentNotNullOrEmpty(nameof(checkName), checkName);
            Guard.ArgumentValid(!_checksByName.ContainsKey(checkName), nameof(checkName), $"A check with name '{checkName}' has already been registered.");

            var namedCheck = CachedHealthCheck.FromType(checkName, cacheDuration, typeof(TCheck));

            _checksByName.Add(checkName, namedCheck);
            _currentGroup.ChecksInternal.Add(namedCheck);

            return this;
        }

        /// <summary>
        /// Registers a concrete health check to the builder.
        /// </summary>
        public HealthCheckBuilder AddCheck(string checkName, IHealthCheck check, TimeSpan cacheDuration)
        {
            Guard.ArgumentNotNullOrEmpty(nameof(checkName), checkName);
            Guard.ArgumentNotNull(nameof(check), check);
            Guard.ArgumentValid(!_checksByName.ContainsKey(checkName), nameof(checkName), $"A check with name '{checkName}' has already been registered.");

            var namedCheck = CachedHealthCheck.FromHealthCheck(checkName, cacheDuration, check);

            _checksByName.Add(checkName, namedCheck);
            _currentGroup.ChecksInternal.Add(namedCheck);

            return this;
        }

        /// <summary>
        /// Creates a new health check group, to which you can add one or more health
        /// checks. Uses <see cref="CheckStatus.Unhealthy"/> when the group is
        /// partially successful.
        /// </summary>
        public HealthCheckBuilder AddHealthCheckGroup(string groupName, Action<HealthCheckBuilder> groupChecks)
            => AddHealthCheckGroup(groupName, groupChecks, CheckStatus.Unhealthy);

        /// <summary>
        /// Creates a new health check group, to which you can add one or more health
        /// checks.
        /// </summary>
        public HealthCheckBuilder AddHealthCheckGroup(string groupName, Action<HealthCheckBuilder> groupChecks, CheckStatus partialSuccessStatus)
        {
            Guard.ArgumentNotNullOrEmpty(nameof(groupName), groupName);
            Guard.ArgumentNotNull(nameof(groupChecks), groupChecks);
            Guard.ArgumentValid(partialSuccessStatus != CheckStatus.Unknown, nameof(partialSuccessStatus), "Check status 'Unknown' is not valid for partial success.");
            Guard.ArgumentValid(!_groups.ContainsKey(groupName), nameof(groupName), $"A group with name '{groupName}' has already been registered.");
            Guard.OperationValid(_currentGroup.GroupName == string.Empty, "Nested groups are not supported by HealthCheckBuilder.");

            var group = new HealthCheckGroup(groupName, partialSuccessStatus);
            _groups.Add(groupName, group);

            var innerBuilder = new HealthCheckBuilder(this, group);
            groupChecks(innerBuilder);

            return this;
        }

        public HealthCheckBuilder WithDefaultCacheDuration(TimeSpan duration)
        {
            Guard.ArgumentValid(duration >= TimeSpan.Zero, nameof(duration), "Duration must be zero (disabled) or a positive duration.");

            DefaultCacheDuration = duration;
            return this;
        }

        public HealthCheckBuilder WithPartialSuccessStatus(CheckStatus partiallyHealthyStatus)
        {
            _currentGroup.PartiallyHealthyStatus = partiallyHealthyStatus;

            return this;
        }
    }
}
