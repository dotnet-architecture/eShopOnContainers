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
    public class OrderStatusChangedToShippedIntegrationEventHandler : IIntegrationEventHandler<OrderStatusChangedToShippedIntegrationEvent>
    {
        private readonly IWebhooksRetriever _retriever;
        private readonly IWebhooksSender _sender;
        private readonly ILogger _logger;
        public OrderStatusChangedToShippedIntegrationEventHandler(IWebhooksRetriever retriever, IWebhooksSender sender, ILogger<OrderStatusChangedToShippedIntegrationEventHandler> logger )
        {
            _retriever = retriever;
            _sender = sender;
            _logger = logger;
        }

        public async Task Handle(OrderStatusChangedToShippedIntegrationEvent @event)
        {
            var subscriptions = await _retriever.GetSubscriptionsOfType(WebhookType.OrderShipped);
            _logger.LogInformation($"Received OrderStatusChangedToShippedIntegrationEvent and got {subscriptions.Count()} subscriptions to process");
            var whook = new WebhookData(WebhookType.OrderShipped, @event);
            await _sender.SendAll(subscriptions, whook);
        }
    }
}
