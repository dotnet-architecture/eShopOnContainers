using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using HttpClient = System.Net.Http.HttpClient;
using JsonConvert = Newtonsoft.Json.JsonConvert;
using StringContent = System.Net.Http.StringContent;

namespace Microsoft.eShopOnContainers.Web.Shopping.HttpAggregator.Services
{
	using BasketData = Models.BasketData;
	using BasketOperations = Config.UrlsConfig.BasketOperations;
	using UrlsConfig = Config.UrlsConfig;

	public class BasketService : IBasketService
	{

		private readonly HttpClient _apiClient;
		private readonly ILogger<BasketService> _logger;
		private readonly UrlsConfig _urls;

		public BasketService(HttpClient httpClient, ILogger<BasketService> logger, IOptions<UrlsConfig> config)
		{
			_apiClient = httpClient;
			_logger = logger;
			_urls = config.Value;
		}

		public async Task<BasketData> GetById(string id)
		{
			var data = await _apiClient.GetStringAsync(_urls.Basket + BasketOperations.GetItemById(id));
			var basket = !string.IsNullOrEmpty(data) ? JsonConvert.DeserializeObject<BasketData>(data) : null;
			return basket;
		}

		public async Task Update(BasketData currentBasket)
		{
			var basketContent = new StringContent(JsonConvert.SerializeObject(currentBasket), System.Text.Encoding.UTF8, "application/json");

			var data = await _apiClient.PostAsync(_urls.Basket + BasketOperations.UpdateBasket(), basketContent);
		}
	}
}
