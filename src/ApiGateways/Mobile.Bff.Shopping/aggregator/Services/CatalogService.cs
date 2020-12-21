using CatalogApi;
using Microsoft.eShopOnContainers.Mobile.Shopping.HttpAggregator.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.eShopOnContainers.Mobile.Shopping.HttpAggregator.Services
{
    public class CatalogService : ICatalogService
    {
        private readonly Catalog.CatalogClient _client;

        public CatalogService(Catalog.CatalogClient client)
        {
            _client = client;
        }

        public async Task<CatalogItem> GetCatalogItemAsync(int id)
        {
            var request = new CatalogItemRequest { Id = id };
            var response = await _client.GetItemByIdAsync(request);
            return MapToCatalogItemResponse(response);
        }

        public async Task<IEnumerable<CatalogItem>> GetCatalogItemsAsync(IEnumerable<int> ids)
        {
            var request = new CatalogItemsRequest { Ids = string.Join(",", ids), PageIndex = 1, PageSize = 10 };
            var response = await _client.GetItemsByIdsAsync(request);
            return response.Data.Select(MapToCatalogItemResponse);
        }

        private CatalogItem MapToCatalogItemResponse(CatalogItemResponse catalogItemResponse)
        {
            return new CatalogItem
            {
                Id = catalogItemResponse.Id,
                Name = catalogItemResponse.Name,
                PictureUri = catalogItemResponse.PictureUri,
                Price = (decimal)catalogItemResponse.Price
            };
        }
    }
}
