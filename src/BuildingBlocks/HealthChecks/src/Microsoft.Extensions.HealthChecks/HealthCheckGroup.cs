// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace Microsoft.Extensions.HealthChecks
{
    public class HealthCheckGroup
    {
        private CheckStatus _partialSuccessStatus;

        public HealthCheckGroup(string groupName, CheckStatus partialSuccessStatus)
        {
            Guard.ArgumentNotNull(nameof(groupName), groupName);

            GroupName = groupName;
            PartiallyHealthyStatus = partialSuccessStatus;
        }

        public IReadOnlyList<CachedHealthCheck> Checks => ChecksInternal.AsReadOnly();

        internal List<CachedHealthCheck> ChecksInternal { get; } = new List<CachedHealthCheck>();

        public string GroupName { get; }

        public CheckStatus PartiallyHealthyStatus
        {
            get => _partialSuccessStatus;
            internal set
            {
                Guard.ArgumentValid(value != CheckStatus.Unknown, nameof(value), "Check status 'Unknown' is not valid for partial success.");

                _partialSuccessStatus = value;
            }
        }
    }
}
