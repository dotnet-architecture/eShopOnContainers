using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.eShopOnContainers.Services.Common.Infrastructure
{
    public interface IIntegrationEvent
    {
        string Name { get; }
    }
}
