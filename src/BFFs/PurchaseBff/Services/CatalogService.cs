using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.eShopOnContainers.BuildingBlocks.Resilience.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PurchaseBff.Config;
using PurchaseBff.Models;

namespace PurchaseBff.Services
{
    public class CatalogService : ICatalogService
    {

        private readonly IHttpClient _apiClient;
        private readonly ILogger<CatalogService> _logger;
        private readonly UrlsConfig _urls;

        public CatalogService(IHttpClient httpClient, ILogger<CatalogService> logger, IOptionsSnapshot<UrlsConfig> config)
        {
            _apiClient = httpClient;
            _logger = logger;
            _urls = config.Value;
        }

        public async Task<CatalogItem> GetCatalogItem(int id)
        {
            var data = await _apiClient.GetStringAsync(_urls.Catalog + UrlsConfig.CatalogOperations.GetItemById(id));
            var item = JsonConvert.DeserializeObject<CatalogItem>(data);
            return item;
        }
    }
}
