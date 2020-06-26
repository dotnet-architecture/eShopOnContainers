using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webhooks.API.Model;
using Webhooks.API.Services;
using Microsoft.Extensions.Logging;

namespace Webhooks.API.IntegrationEvents
{
    public class OrderStatusChangedToPaidIntegrationEventHandler : IIntegrationEventHandler<OrderStatusChangedToPaidIntegrationEvent>
    {
        private readonly IWebhooksRetriever _retriever;
        private readonly IWebhooksSender _sender;
        private readonly ILogger _logger;
        public OrderStatusChangedToPaidIntegrationEventHandler(IWebhooksRetriever retriever, IWebhooksSender sender, ILogger<OrderStatusChangedToShippedIntegrationEventHandler> logger )
        {
            _retriever = retriever;
            _sender = sender;
            _logger = logger;
        }

        public async Task Handle(OrderStatusChangedToPaidIntegrationEvent @event)
        {
            var subscriptions = await _retriever.GetSubscriptionsOfType(WebhookType.OrderPaid);
            _logger.LogInformation("Received OrderStatusChangedToShippedIntegrationEvent and got {SubscriptionsCount} subscriptions to process", subscriptions.Count());
            var whook = new WebhookData(WebhookType.OrderPaid, @event);
            await _sender.SendAll(subscriptions, whook);
        }
    }
}
