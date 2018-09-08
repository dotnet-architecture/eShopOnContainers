using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.eShopOnContainers.Mobile.Shopping.HttpAggregator.Services
{
	using CatalogItem = Models.CatalogItem;

	public interface ICatalogService
	{
		Task<CatalogItem> GetCatalogItem(int id);
		Task<IEnumerable<CatalogItem>> GetCatalogItems(IEnumerable<int> ids);
	}
}
