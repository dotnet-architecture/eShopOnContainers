using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using eShopOnContainers.Core.Models.Catalog;
using eShopOnContainers.Core.Services.RequestProvider;
using System.Collections.Generic;
using eShopOnContainers.Core.Extensions;

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

            builder.Path = "/items";

            string uri = builder.ToString();

            IEnumerable<CatalogItem> catalogItems = 
                await _requestProvider.GetAsync<IEnumerable<CatalogItem>>(uri);

            return catalogItems.ToObservableCollection();
        }

        public Task<ObservableCollection<CatalogBrand>> GetCatalogBrandAsync()
        {
            throw new NotImplementedException();
        }

        public Task<CatalogItem> GetCatalogItemAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<ObservableCollection<CatalogType>> GetCatalogTypeAsync()
        {
            throw new NotImplementedException();
        }
    }
}
