// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.HealthChecks
{
    public interface IHealthCheckService
    {
        /// <summary>
        /// Runs all registered health checks.
        /// </summary>
        Task<CompositeHealthCheckResult> CheckHealthAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets all registered health checks as a flat list.
        /// </summary>
        IReadOnlyList<CachedHealthCheck> GetAllChecks();

        /// <summary>
        /// Gets a health check by name.
        /// </summary>
        CachedHealthCheck GetCheck(string checkName);

        /// <summary>
        /// Gets all health checks in a group.
        /// </summary>
        HealthCheckGroup GetGroup(string groupName);

        /// <summary>
        /// Gets all the health check groups.
        /// </summary>
        IReadOnlyList<HealthCheckGroup> GetGroups();

        /// <summary>
        /// Creates a new resolution scope from the default service provider and executes the provided check.
        /// </summary>
        ValueTask<IHealthCheckResult> RunCheckAsync(CachedHealthCheck healthCheck, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Uses the provided service provider and executes the provided check.
        /// </summary>
        ValueTask<IHealthCheckResult> RunCheckAsync(IServiceProvider serviceProvider, CachedHealthCheck healthCheck, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Creates a new resolution scope from the default service provider and executes the checks in the given group.
        /// </summary>
        Task<CompositeHealthCheckResult> RunGroupAsync(HealthCheckGroup group, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Uses the provided service provider and executes the checks in the given group.
        /// </summary>
        Task<CompositeHealthCheckResult> RunGroupAsync(IServiceProvider serviceProvider, HealthCheckGroup group, CancellationToken cancellationToken = default(CancellationToken));
    }
}
