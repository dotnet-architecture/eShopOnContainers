using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.eShopOnContainers.Services.Common.Infrastructure
{
    public interface IEventBus
    {
        void Subscribe<T>(IIntegrationEventHandler<T> handler) where T: IIntegrationEvent;
        void Unsubscribe<T>(IIntegrationEventHandler<T> handler) where T : IIntegrationEvent;
        void Publish(IIntegrationEvent @event);
    }
}
