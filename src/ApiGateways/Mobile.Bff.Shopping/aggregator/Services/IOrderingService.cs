namespace Microsoft.eShopOnContainers.Mobile.Shopping.HttpAggregator.Services;

public interface IOrderingService
{
    Task<OrderData> GetOrderDraftAsync(BasketData basketData);
}
