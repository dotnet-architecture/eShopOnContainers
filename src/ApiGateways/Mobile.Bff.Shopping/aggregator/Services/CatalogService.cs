using Microsoft.eShopOnContainers.Mobile.Shopping.HttpAggregator.Config;
using Microsoft.eShopOnContainers.Mobile.Shopping.HttpAggregator.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.eShopOnContainers.Mobile.Shopping.HttpAggregator.Services
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

        public async Task<CatalogItem> GetCatalogItem(int id)
        {
            var stringContent = await _httpClient.GetStringAsync(_urls.Catalog + UrlsConfig.CatalogOperations.GetItemById(id));
            var catalogItem = JsonConvert.DeserializeObject<CatalogItem>(stringContent);

            return catalogItem;
        }

        public async Task<IEnumerable<CatalogItem>> GetCatalogItems(IEnumerable<int> ids)
        {
            var stringContent = await _httpClient.GetStringAsync(_urls.Catalog + UrlsConfig.CatalogOperations.GetItemsById(ids));
            var catalogItems = JsonConvert.DeserializeObject<CatalogItem[]>(stringContent);

            return catalogItems;
        }
    }
}
