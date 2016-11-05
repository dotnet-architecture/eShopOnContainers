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

namespace Microsoft.eShopOnContainers.WebMVC.Services
{
    public class CatalogService : ICatalogService
    {
        private readonly List<CatalogItem> _items;          //Fake data while services are ready. 
        private readonly IOptions<AppSettings> _settings;
        private HttpClient _apiClient;
        private readonly string _remoteServiceBaseUrl;
        private int _totalItems;

        public int TotalItems 
        {
            get
            {
                return _totalItems;
            }
        }

        public CatalogService(IOptions<AppSettings> settings) {
            _settings = settings;
            _remoteServiceBaseUrl = $"{_settings.Value.CatalogUrl}api/v1/catalog/";

            #region fake data
            _items = new List<CatalogItem>()
            {
                new CatalogItem() { Id = Guid.NewGuid().ToString(), Description = "Roslyn Red T-Shirt", Name = "Roslyn Red T-Shirt", Price = 12, PictureUri = "https://fakeimg.pl/370x240/EEEEEE/000/?text=RoslynRedT-Shirt"  },
                new CatalogItem() { Id = Guid.NewGuid().ToString(), Description = "Cupt Black & White Mug", Name = "Cupt Black & White Mug", Price= 17, PictureUri = "https://fakeimg.pl/370x240/EEEEEE/000/?text=CuptBlack&WhiteMug" },
                new CatalogItem() { Id = Guid.NewGuid().ToString(), Description = "Prism White T-Shirt", Name = "Prism White T-Shirt", Price = 12, PictureUri = "http://fakeimg.pl/370x240/EEEEEE/000/?text=.PrismWhiteT-Shirt" },
                new CatalogItem() { Id = Guid.NewGuid().ToString(), Description = ".NET Bot Black Sweatshirt", Name = ".NET Bot Black Sweatshirt", Price = decimal.Parse("19.5"), PictureUri = "http://fakeimg.pl/370x240/EEEEEE/000/?text=.NETBotBlack" },
                new CatalogItem() { Id = Guid.NewGuid().ToString(), Description = "Roslyn Red T-Shirt", Name = "Roslyn Red T-Shirt", Price = 12, PictureUri = "http://fakeimg.pl/370x240/EEEEEE/000/?text=RoslynRedT-Shirt"  },
                new CatalogItem() { Id = Guid.NewGuid().ToString(), Description = "Cupt Black & White Mug", Name = "Cupt Black & White Mug", Price= 17, PictureUri = "https://fakeimg.pl/370x240/EEEEEE/000/?text=CuptBlack&WhiteMug" },
                new CatalogItem() { Id = Guid.NewGuid().ToString(), Description = "Prism White T-Shirt", Name = "Prism White T-Shirt", Price = 12, PictureUri = "http://fakeimg.pl/370x240/EEEEEE/000/?text=.PrismWhiteT-Shirt" },
                new CatalogItem() { Id = Guid.NewGuid().ToString(), Description = ".NET Bot Black Sweatshirt", Name = ".NET Bot Black Sweatshirt", Price = decimal.Parse("19.5"), PictureUri = "http://fakeimg.pl/370x240/EEEEEE/000/?text=.NETBotBlack" },
                new CatalogItem() { Id = Guid.NewGuid().ToString(), Description = "Roslyn Red T-Shirt", Name = "Roslyn Red T-Shirt", Price = 12, PictureUri = "http://fakeimg.pl/370x240/EEEEEE/000/?text=RoslynRedT-Shirt"  },
                new CatalogItem() { Id = Guid.NewGuid().ToString(), Description = "Cupt Black & White Mug", Name = "Cupt Black & White Mug", Price= 17, PictureUri = "https://fakeimg.pl/370x240/EEEEEE/000/?text=CuptBlack&WhiteMug" },
                new CatalogItem() { Id = Guid.NewGuid().ToString(), Description = "Prism White T-Shirt", Name = "Prism White T-Shirt", Price = 12, PictureUri = "http://fakeimg.pl/370x240/EEEEEE/000/?text=.PrismWhiteT-Shirt" },
                new CatalogItem() { Id = Guid.NewGuid().ToString(), Description = ".NET Bot Black Sweatshirt", Name = ".NET Bot Black Sweatshirt", Price = decimal.Parse("19.5"), PictureUri = "http://fakeimg.pl/370x240/EEEEEE/000/?text=.NETBotBlack" },
                new CatalogItem() { Id = Guid.NewGuid().ToString(), Description = "Roslyn Red T-Shirt", Name = "Roslyn Red T-Shirt", Price = 12, PictureUri = "http://fakeimg.pl/370x240/EEEEEE/000/?text=RoslynRedT-Shirt"  },
                new CatalogItem() { Id = Guid.NewGuid().ToString(), Description = "Cupt Black & White Mug", Name = "Cupt Black & White Mug", Price= 17, PictureUri = "https://fakeimg.pl/370x240/EEEEEE/000/?text=CuptBlack&WhiteMug" },
                new CatalogItem() { Id = Guid.NewGuid().ToString(), Description = "Prism White T-Shirt", Name = "Prism White T-Shirt", Price = 12, PictureUri = "http://fakeimg.pl/370x240/EEEEEE/000/?text=.PrismWhiteT-Shirt" },
                new CatalogItem() { Id = Guid.NewGuid().ToString(), Description = ".NET Bot Black Sweatshirt", Name = ".NET Bot Black Sweatshirt", Price = decimal.Parse("19.5"), PictureUri = "http://fakeimg.pl/370x240/EEEEEE/000/?text=.NETBotBlack" },
                new CatalogItem() { Id = Guid.NewGuid().ToString(), Description = "Roslyn Red T-Shirt", Name = "Roslyn Red T-Shirt", Price = 12, PictureUri = "http://fakeimg.pl/370x240/EEEEEE/000/?text=RoslynRedT-Shirt"  },
                new CatalogItem() { Id = Guid.NewGuid().ToString(), Description = "Cupt Black & White Mug", Name = "Cupt Black & White Mug", Price= 17, PictureUri = "https://fakeimg.pl/370x240/EEEEEE/000/?text=CuptBlack&WhiteMug" },
                new CatalogItem() { Id = Guid.NewGuid().ToString(), Description = "Prism White T-Shirt", Name = "Prism White T-Shirt", Price = 12, PictureUri = "http://fakeimg.pl/370x240/EEEEEE/000/?text=.PrismWhiteT-Shirt" },
                new CatalogItem() { Id = Guid.NewGuid().ToString(), Description = ".NET Bot Black Sweatshirt", Name = ".NET Bot Black Sweatshirt", Price = decimal.Parse("19.5"), PictureUri = "http://fakeimg.pl/370x240/EEEEEE/000/?text=.NETBotBlack" },
                new CatalogItem() { Id = Guid.NewGuid().ToString(), Description = "Roslyn Red T-Shirt", Name = "Roslyn Red T-Shirt", Price = 12, PictureUri = "http://fakeimg.pl/370x240/EEEEEE/000/?text=RoslynRedT-Shirt"  },
                new CatalogItem() { Id = Guid.NewGuid().ToString(), Description = "Cupt Black & White Mug", Name = "Cupt Black & White Mug", Price= 17, PictureUri = "https://fakeimg.pl/370x240/EEEEEE/000/?text=CuptBlack&WhiteMug" },
                new CatalogItem() { Id = Guid.NewGuid().ToString(), Description = "Prism White T-Shirt", Name = "Prism White T-Shirt", Price = 12, PictureUri = "http://fakeimg.pl/370x240/EEEEEE/000/?text=.PrismWhiteT-Shirt" },
                new CatalogItem() { Id = Guid.NewGuid().ToString(), Description = ".NET Bot Black Sweatshirt", Name = ".NET Bot Black Sweatshirt", Price = decimal.Parse("19.5"), PictureUri = "http://fakeimg.pl/370x240/EEEEEE/000/?text=.NETBotBlack" },
                new CatalogItem() { Id = Guid.NewGuid().ToString(), Description = "Roslyn Red T-Shirt", Name = "Roslyn Red T-Shirt", Price = 12, PictureUri = "http://fakeimg.pl/370x240/EEEEEE/000/?text=RoslynRedT-Shirt"  },
                new CatalogItem() { Id = Guid.NewGuid().ToString(), Description = "Cupt Black & White Mug", Name = "Cupt Black & White Mug", Price= 17, PictureUri = "https://fakeimg.pl/370x240/EEEEEE/000/?text=CuptBlack&WhiteMug" },
                new CatalogItem() { Id = Guid.NewGuid().ToString(), Description = "Prism White T-Shirt", Name = "Prism White T-Shirt", Price = 12, PictureUri = "http://fakeimg.pl/370x240/EEEEEE/000/?text=.PrismWhiteT-Shirt" },
                new CatalogItem() { Id = Guid.NewGuid().ToString(), Description = ".NET Bot Black Sweatshirt", Name = ".NET Bot Black Sweatshirt", Price = decimal.Parse("19.5"), PictureUri = "http://fakeimg.pl/370x240/EEEEEE/000/?text=.NETBotBlack" }
            };
            #endregion
        }
         
        public CatalogItem GetCatalogItem(string Id)
        {
            return _items.Where(x => x.Id.Equals(Id)).FirstOrDefault();
        }

        public async Task<Catalog> GetCatalogItems(int page,int take, int? brand, int? type)
        {
            _apiClient = new HttpClient();
            var itemsQs = $"items?pageIndex={page}&pageSize={take}";
            var filterQs = "";

            if (brand.HasValue || type.HasValue)
                filterQs = $"/type/{type ?? null}/brand/{brand ?? null}";

            var catalogUrl = $"{_remoteServiceBaseUrl}items{filterQs}?pageIndex={page}&pageSize={take}";
            var dataString = await _apiClient.GetStringAsync(catalogUrl);
            var response = JsonConvert.DeserializeObject<Catalog>(dataString);

            var res = _items;
            _totalItems = response.Count;

            return response;
        }

        public async Task<IEnumerable<SelectListItem>> GetBrands()
        {
            _apiClient = new HttpClient();
            var url = $"{_remoteServiceBaseUrl}catalogBrands";
            var dataString = await _apiClient.GetStringAsync(url);

            var items = new List<SelectListItem>();
            items.Add(new SelectListItem() { Value = "0", Text = "All", Selected = true });

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
            items.Add(new SelectListItem() { Value = "0", Text = "All", Selected = true });

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
