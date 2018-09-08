using System.Threading.Tasks;

namespace Microsoft.eShopOnContainers.Web.Shopping.HttpAggregator.Services
{
	using BasketData = Models.BasketData;

	public interface IBasketService
	{
		Task<BasketData> GetById(string id);
		Task Update(BasketData currentBasket);

	}
}
