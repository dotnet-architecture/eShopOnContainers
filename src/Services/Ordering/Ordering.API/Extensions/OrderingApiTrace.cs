namespace Microsoft.eShopOnContainers.Services.Ordering.API.Application.DomainEventHandlers;

internal static partial class OrderingApiTrace
{
    [LoggerMessage(EventId = 1, EventName = "OrderStatusUpdated", Level = LogLevel.Trace, Message = "Order with Id: {OrderId} has been successfully updated to status {Status} ({Id})")]
    public static partial void LogOrderStatusUpdated(ILogger logger, int orderId, string status, int id);

    [LoggerMessage(EventId = 2, EventName = "PaymentMethodUpdated", Level = LogLevel.Trace, Message = "Order with Id: {OrderId} has been successfully updated with a payment method {PaymentMethod} ({Id})")]
    public static partial void LogOrderPaymentMethodUpdated(ILogger logger, int orderId, string paymentMethod, int id);

    [LoggerMessage(EventId = 3, EventName = "BuyerAndPaymentValidatedOrUpdated", Level = LogLevel.Trace, Message = "Buyer {BuyerId} and related payment method were validated or updated for order Id: {OrderId}.")]
    public static partial void LogOrderBuyerAndPaymentValidatedOrUpdated(ILogger logger, int buyerId, int orderId);
}
