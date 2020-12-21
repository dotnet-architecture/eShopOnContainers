using CatalogApi;
using Microsoft.eShopOnContainers.Web.Shopping.HttpAggregator.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.eShopOnContainers.Web.Shopping.HttpAggregator.Services
{
    public class CatalogService : ICatalogService
    {
        private readonly Catalog.CatalogClient _client;
        private readonly ILogger<CatalogService> _logger;

        public CatalogService(Catalog.CatalogClient client, ILogger<CatalogService> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task<CatalogItem> GetCatalogItemAsync(int id)
        {
            var request = new CatalogItemRequest { Id = id };
            _logger.LogInformation("grpc request {@request}", request);
            var response = await _client.GetItemByIdAsync(request);
            _logger.LogInformation("grpc response {@response}", response);
            return MapToCatalogItemResponse(response);

        }

        public async Task<IEnumerable<CatalogItem>> GetCatalogItemsAsync(IEnumerable<int> ids)
        {
            var request = new CatalogItemsRequest { Ids = string.Join(",", ids), PageIndex = 1, PageSize = 10 };
            _logger.LogInformation("grpc request {@request}", request);
            var response = await _client.GetItemsByIdsAsync(request);
            _logger.LogInformation("grpc response {@response}", response);
            return response.Data.Select(this.MapToCatalogItemResponse);

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
