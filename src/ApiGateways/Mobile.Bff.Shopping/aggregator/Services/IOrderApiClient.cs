using System.Threading.Tasks;

namespace Microsoft.eShopOnContainers.Mobile.Shopping.HttpAggregator.Services
{
	using BasketData = Models.BasketData;
	using OrderData = Models.OrderData;

	public interface IOrderApiClient
	{
		Task<OrderData> GetOrderDraftFromBasket(BasketData basket);
	}
}
