namespace Microsoft.eShopOnContainers.Web.Shopping.HttpAggregator.Services;

public interface IOrderingService
{
    Task<OrderData> GetOrderDraftAsync(BasketData basketData);
    Task<CompleteData> CompleteOrderAsync(string orderId);
}
