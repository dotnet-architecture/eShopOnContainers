namespace Microsoft.eShopOnContainers.Web.Shopping.HttpAggregator.Services;

public interface IOrderApiClient
{
    Task<OrderData> GetOrderDraftFromBasketAsync(BasketData basket);
}
