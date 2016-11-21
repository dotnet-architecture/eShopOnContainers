using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using eShopOnContainers.Core.Models.Catalog;
using eShopOnContainers.Core.Services.RequestProvider;
using eShopOnContainers.Core.Extensions;

namespace eShopOnContainers.Core.Services.Catalog
{
    public class CatalogService : ICatalogService
    {
        private readonly IRequestProvider _requestProvider;

        private ObservableCollection<CatalogBrand> MockCatalogBrand = new ObservableCollection<CatalogBrand>
        {
            new CatalogBrand { CatalogBrandId = 1, Name = "Azure" },
            new CatalogBrand { CatalogBrandId = 2, Name = "Visual Studio" }
        };

        private ObservableCollection<CatalogType> MockCatalogType = new ObservableCollection<CatalogType>
        {
            new CatalogType { CatalogTypeId = 1, Name = "Mug" },
            new CatalogType { CatalogTypeId = 2, Name = "T-Shirt" }
        };

        public CatalogService(IRequestProvider requestProvider)
        {
            _requestProvider = requestProvider;
        }

        public Task<ObservableCollection<CatalogItem>> FilterAsync(string catalogBrand, string catalogType)
        {
            throw new NotImplementedException();
        }

        public async Task<ObservableCollection<CatalogItem>> GetCatalogAsync()
        {
            UriBuilder builder = new UriBuilder(GlobalSetting.CatalogEndpoint);

            builder.Path = "api/v1/catalog/items";

            string uri = builder.ToString();

            System.Diagnostics.Debug.WriteLine(uri);

            CatalogRoot catalog = 
                await _requestProvider.GetAsync<CatalogRoot>(uri);

            return catalog?.Data?.ToObservableCollection();
        }

        public Task<CatalogItem> GetCatalogItemAsync(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<ObservableCollection<CatalogBrand>> GetCatalogBrandAsync()
        {
            await Task.Delay(500);

            return MockCatalogBrand;
        }

        public async Task<ObservableCollection<CatalogType>> GetCatalogTypeAsync()
        {
            await Task.Delay(500);

            return MockCatalogType;
        }
    }
}
