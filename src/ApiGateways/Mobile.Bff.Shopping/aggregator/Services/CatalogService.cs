using CatalogApi;
using Grpc.Net.Client;
using Microsoft.eShopOnContainers.Mobile.Shopping.HttpAggregator.Config;
using Microsoft.eShopOnContainers.Mobile.Shopping.HttpAggregator.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using static CatalogApi.Catalog;

namespace Microsoft.eShopOnContainers.Mobile.Shopping.HttpAggregator.Services
{
    public class CatalogService : ICatalogService
    {
        private readonly HttpClient _httpClient;
        private readonly UrlsConfig _urls;

        public CatalogService(HttpClient httpClient, IOptions<UrlsConfig> config)
        {
            _httpClient = httpClient;
            _urls = config.Value;
        }

        public async Task<CatalogItem> GetCatalogItemAsync(int id)
        {
            _httpClient.BaseAddress = new Uri(_urls.Catalog + UrlsConfig.CatalogOperations.GetItemById(id));

            var client = GrpcClient.Create<CatalogClient>(_httpClient);
            var request = new CatalogItemRequest { Id = id };
            var response = await client.GetItemByIdAsync(request);

            return MapToCatalogItemResponse(response);
        }

        public async Task<IEnumerable<CatalogItem>> GetCatalogItemsAsync(IEnumerable<int> ids)
        {
            var stringContent = await _httpClient.GetStringAsync(_urls.Catalog + UrlsConfig.CatalogOperations.GetItemsById(ids));
            var catalogItems = JsonConvert.DeserializeObject<CatalogItem[]>(stringContent);

            return catalogItems;
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
