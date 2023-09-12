namespace Webhooks.API.IntegrationEvents;

public class OrderStatusChangedToCompletedIntegrationEventHandler : IIntegrationEventHandler<OrderStatusChangedToCompletedIntegrationEvent>
{
    private readonly IWebhooksRetriever _retriever;
    private readonly IWebhooksSender _sender;
    private readonly ILogger _logger;
    public OrderStatusChangedToCompletedIntegrationEventHandler(IWebhooksRetriever retriever, IWebhooksSender sender, ILogger<OrderStatusChangedToCompletedIntegrationEventHandler> logger)
    {
        _retriever = retriever;
        _sender = sender;
        _logger = logger;
    }

    public async Task Handle(OrderStatusChangedToCompletedIntegrationEvent @event)
    {
        var subscriptions = await _retriever.GetSubscriptionsOfType(WebhookType.OrderShipped);
        _logger.LogInformation("Received OrderStatusChangedToShippedIntegrationEvent and got {SubscriptionCount} subscriptions to process", subscriptions.Count());
        var whook = new WebhookData(WebhookType.OrderShipped, @event);
        await _sender.SendAll(subscriptions, whook);
    }
}
