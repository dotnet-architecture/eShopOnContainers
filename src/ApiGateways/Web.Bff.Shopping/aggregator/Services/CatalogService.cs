using Microsoft.eShopOnContainers.Web.Shopping.HttpAggregator.Config;
using Microsoft.eShopOnContainers.Web.Shopping.HttpAggregator.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using CatalogApi;
using Grpc.Net.Client;
using System;
using static CatalogApi.Catalog;
using System.Linq;

namespace Microsoft.eShopOnContainers.Web.Shopping.HttpAggregator.Services
{
    public class CatalogService : ICatalogService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<CatalogService> _logger;
        private readonly UrlsConfig _urls;

        public CatalogService(HttpClient httpClient, ILogger<CatalogService> logger, IOptions<UrlsConfig> config)
        {
            _httpClient = httpClient;
            _logger = logger;
            _urls = config.Value;
        }

        public async Task<CatalogItem> GetCatalogItemAsync(int id)
        {
            return await GrpcCallerService.CallService(_urls.GrpcCatalog, async channel =>
            {
                var client = new CatalogClient(channel);
                var request = new CatalogItemRequest { Id = id };
                _logger.LogInformation("grpc client created, request = {@request}", request);
                var response = await client.GetItemByIdAsync(request);
                _logger.LogInformation("grpc response {@response}", response);
                return MapToCatalogItemResponse(response);
            });
        }

        public async Task<IEnumerable<CatalogItem>> GetCatalogItemsAsync(IEnumerable<int> ids)
        {
            return await GrpcCallerService.CallService(_urls.GrpcCatalog, async channel =>
            {
                var client = new CatalogClient(channel);
                var request = new CatalogItemsRequest { Ids = string.Join(",", ids), PageIndex = 1, PageSize = 10 };
                _logger.LogInformation("grpc client created, request = {@request}", request);
                var response = await client.GetItemsByIdsAsync(request);
                _logger.LogInformation("grpc response {@response}", response);
                return response.Data.Select(this.MapToCatalogItemResponse);
            });
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
