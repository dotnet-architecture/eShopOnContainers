using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webhooks.API.Model;
using Webhooks.API.Services;

namespace Webhooks.API.IntegrationEvents
{
    public class OrderStatusChangedToShippedIntegrationEventHandler : IIntegrationEventHandler<OrderStatusChangedToShippedIntegrationEvent>
    {
        private readonly IWebhooksRetriever _retriever;
        private readonly IWebhooksSender _sender;
        public OrderStatusChangedToShippedIntegrationEventHandler(IWebhooksRetriever retriever, IWebhooksSender sender )
        {
            _retriever = retriever;
            _sender = sender;
        }

        public async Task Handle(OrderStatusChangedToShippedIntegrationEvent @event)
        {
            var subscriptions = await _retriever.GetSubscriptionsOfType(WebhookType.OrderShipped);

            var whook = new WebhookData(WebhookType.OrderShipped, @event);
            await _sender.SendAll(subscriptions, whook);
        }
    }
}
