using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.eShopOnContainers.Services.Common.Infrastructure
{
    public interface IIntegrationEventHandler<in TIntegrationEvent> : IIntegrationEventHandler 
        where TIntegrationEvent: IIntegrationEvent
    {
        void Handle(TIntegrationEvent @event);
    }

    public interface IIntegrationEventHandler
    {
    }
}
