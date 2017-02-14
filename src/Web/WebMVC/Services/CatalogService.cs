using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.eShopOnContainers.WebMVC.Models;
using Microsoft.CodeAnalysis.Options;
using Microsoft.Extensions.Options;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Logging;

namespace Microsoft.eShopOnContainers.WebMVC.Services
{
    public class CatalogService : ICatalogService
    {
        private readonly IOptionsSnapshot<AppSettings> _settings;
        private HttpClient _apiClient;
        private readonly string _remoteServiceBaseUrl;
  
        public CatalogService(IOptionsSnapshot<AppSettings> settings, ILoggerFactory loggerFactory) {
            _settings = settings;
            _remoteServiceBaseUrl = $"{_settings.Value.CatalogUrl}/api/v1/catalog/";

            var log = loggerFactory.CreateLogger("catalog service");
            log.LogDebug(settings.Value.CatalogUrl);
        }
         
        public async Task<Catalog> GetCatalogItems(int page,int take, int? brand, int? type)
        {
            _apiClient = new HttpClient();
            var itemsQs = $"items?pageIndex={page}&pageSize={take}";
            var filterQs = "";

            if (brand.HasValue || type.HasValue)
            {
                var brandQs = (brand.HasValue) ? brand.Value.ToString() : "null";
                var typeQs = (type.HasValue) ? type.Value.ToString() : "null";
                filterQs = $"/type/{typeQs}/brand/{brandQs}";
            }

            var catalogUrl = $"{_remoteServiceBaseUrl}items{filterQs}?pageIndex={page}&pageSize={take}";
            var dataString = await _apiClient.GetStringAsync(catalogUrl);
            var response = JsonConvert.DeserializeObject<Catalog>(dataString);

            return response;
        }

        public async Task<IEnumerable<SelectListItem>> GetBrands()
        {
            _apiClient = new HttpClient();
            var url = $"{_remoteServiceBaseUrl}catalogBrands";
            var dataString = await _apiClient.GetStringAsync(url);

            var items = new List<SelectListItem>();
            items.Add(new SelectListItem() { Value = null, Text = "All", Selected = true });

            JArray brands = JArray.Parse(dataString);
            foreach (JObject brand in brands.Children<JObject>())
            {
                dynamic item = brand;
                items.Add(new SelectListItem() { Value = item.id, Text = item.brand });
            }

            return items;
        }

        public async Task<IEnumerable<SelectListItem>> GetTypes()
        {
            _apiClient = new HttpClient();
            var url = $"{_remoteServiceBaseUrl}catalogTypes";
            var dataString = await _apiClient.GetStringAsync(url);

            var items = new List<SelectListItem>();
            items.Add(new SelectListItem() { Value = null, Text = "All", Selected = true });

            JArray brands = JArray.Parse(dataString);
            foreach (JObject brand in brands.Children<JObject>())
            {
                dynamic item = brand;
                items.Add(new SelectListItem() { Value = item.id, Text = item.type });
            }

            return items;
        }
    }
}
