using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Encoding = System.Text.Encoding;
using HttpClient = System.Net.Http.HttpClient;
using JsonConvert = Newtonsoft.Json.JsonConvert;
using StringContent = System.Net.Http.StringContent;

namespace Microsoft.eShopOnContainers.Mobile.Shopping.HttpAggregator.Services
{
	using BasketData = Models.BasketData;
	using OrdersOperations = Config.UrlsConfig.OrdersOperations;
	using OrderData = Models.OrderData;
	using UrlsConfig = Config.UrlsConfig;

	public class OrderApiClient : IOrderApiClient
	{
		private readonly HttpClient _apiClient;
		private readonly ILogger<OrderApiClient> _logger;
		private readonly UrlsConfig _urls;

		public OrderApiClient(HttpClient httpClient, ILogger<OrderApiClient> logger, IOptions<UrlsConfig> config)
		{
			_apiClient = httpClient;
			_logger = logger;
			_urls = config.Value;
		}

		public async Task<OrderData> GetOrderDraftFromBasket(BasketData basket)
		{
			var uri = _urls.Orders + OrdersOperations.GetOrderDraft();
			var content = new StringContent(JsonConvert.SerializeObject(basket), Encoding.UTF8, "application/json");
			var response = await _apiClient.PostAsync(uri, content);

			response.EnsureSuccessStatusCode();

			var ordersDraftResponse = await response.Content.ReadAsStringAsync();

			return JsonConvert.DeserializeObject<OrderData>(ordersDraftResponse);
		}
	}
}
