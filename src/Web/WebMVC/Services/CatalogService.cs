using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.eShopOnContainers.BuildingBlocks.Resilience.Http;
using Microsoft.eShopOnContainers.WebMVC.ViewModels;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.eShopOnContainers.WebMVC.Services
{
    public class CatalogService : ICatalogService
    {
        private readonly IOptionsSnapshot<AppSettings> _settings;
        private IHttpClient _apiClient;
        private readonly string _remoteServiceBaseUrl;
  
        public CatalogService(IOptionsSnapshot<AppSettings> settings, ILoggerFactory loggerFactory, IHttpClient httpClient) {
            _settings = settings;
            _remoteServiceBaseUrl = $"{_settings.Value.CatalogUrl}/api/v1/catalog/";
            _apiClient = httpClient;
            var log = loggerFactory.CreateLogger("catalog service");
            log.LogDebug(settings.Value.CatalogUrl);
        }
         
        public async Task<Catalog> GetCatalogItems(int page,int take, int? brand, int? type)
        {
            var itemsQs = $"items?pageIndex={page}&pageSize={take}";
            var filterQs = "";

            if (brand.HasValue || type.HasValue)
            {
                var brandQs = (brand.HasValue) ? brand.Value.ToString() : "null";
                var typeQs = (type.HasValue) ? type.Value.ToString() : "null";
                filterQs = $"/type/{typeQs}/brand/{brandQs}";
            }

            var catalogUrl = $"{_remoteServiceBaseUrl}items{filterQs}?pageIndex={page}&pageSize={take}";

            var dataString = "";

            //
            // Using a HttpClient wrapper with Retry and Exponential Backoff
            //
            dataString = await _apiClient.GetStringAsync(catalogUrl);

            var response = JsonConvert.DeserializeObject<Catalog>(dataString);

            return response;
        }

        public async Task<IEnumerable<SelectListItem>> GetBrands()
        {
            var url = $"{_remoteServiceBaseUrl}catalogBrands";
            var dataString = await _apiClient.GetStringAsync(url);

            var items = new List<SelectListItem>();
            items.Add(new SelectListItem() { Value = null, Text = "All", Selected = true });

            JArray brands = JArray.Parse(dataString);
            foreach (JObject brand in brands.Children<JObject>())
            {
                items.Add(new SelectListItem()
                {
                    Value = brand.Value<string>("id"),
                    Text = brand.Value<string>("brand")
                });
            }

            return items;
        }

        public async Task<IEnumerable<SelectListItem>> GetTypes()
        {
            var url = $"{_remoteServiceBaseUrl}catalogTypes";
            var dataString = await _apiClient.GetStringAsync(url);

            var items = new List<SelectListItem>();
            items.Add(new SelectListItem() { Value = null, Text = "All", Selected = true });

            JArray brands = JArray.Parse(dataString);
            foreach (JObject brand in brands.Children<JObject>())
            {
                items.Add(new SelectListItem()
                {
                    Value = brand.Value<string>("id"),
                    Text = brand.Value<string>("type")
                });
            }
            return items;
        }
    }
}
