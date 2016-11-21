using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using eShopOnContainers.Core.Models.Catalog;
using eShopOnContainers.Core.Services.RequestProvider;
using eShopOnContainers.Core.Extensions;
using System.Collections.Generic;

namespace eShopOnContainers.Core.Services.Catalog
{
    public class CatalogService : ICatalogService
    {
        private readonly IRequestProvider _requestProvider;

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
            UriBuilder builder = new UriBuilder(GlobalSetting.CatalogEndpoint);

            builder.Path = "api/v1/catalog/catalogbrands";

            string uri = builder.ToString();

            IEnumerable<CatalogBrand> brands =
                   await _requestProvider.GetAsync<IEnumerable<CatalogBrand>>(uri);

            return brands?.ToObservableCollection();
        }

        public async Task<ObservableCollection<CatalogType>> GetCatalogTypeAsync()
        {
            UriBuilder builder = new UriBuilder(GlobalSetting.CatalogEndpoint);

            builder.Path = "api/v1/catalog/catalogtypes";

            string uri = builder.ToString();

            IEnumerable<CatalogType> types =
                   await _requestProvider.GetAsync<IEnumerable<CatalogType>>(uri);

            return types?.ToObservableCollection();
        }
    }
}
