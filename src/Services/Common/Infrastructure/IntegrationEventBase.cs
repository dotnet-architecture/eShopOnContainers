using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.eShopOnContainers.Services.Common.Infrastructure
{
    public class IntegrationEventBase
    {
        public IntegrationEventBase()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id  { get; private set; }
    }
}
