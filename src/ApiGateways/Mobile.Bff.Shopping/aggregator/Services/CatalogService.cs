using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;
using HttpClient = System.Net.Http.HttpClient;
using JsonConvert = Newtonsoft.Json.JsonConvert;

namespace Microsoft.eShopOnContainers.Mobile.Shopping.HttpAggregator.Services
{
	using CatalogItem = Models.CatalogItem;
	using CatalogOperations = Config.UrlsConfig.CatalogOperations;
	using UrlsConfig = Config.UrlsConfig;

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
			var stringContent = await _httpClient.GetStringAsync(_urls.Catalog + CatalogOperations.GetItemById(id));
			var catalogItem = JsonConvert.DeserializeObject<CatalogItem>(stringContent);

			return catalogItem;
		}

		public async Task<IEnumerable<CatalogItem>> GetCatalogItems(IEnumerable<int> ids)
		{
			var stringContent = await _httpClient.GetStringAsync(_urls.Catalog + CatalogOperations.GetItemsById(ids));
			var catalogItems = JsonConvert.DeserializeObject<CatalogItem[]>(stringContent);

			return catalogItems;
		}
	}
}
