using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using eShopOnContainers.Core.Models.Catalog;
using eShopOnContainers.Core.Services.RequestProvider;
using eShopOnContainers.Core.Extensions;
using System.Collections.Generic;
using eShopOnContainers.Core.Services.FixUri;

namespace eShopOnContainers.Core.Services.Catalog
{
    public class CatalogService : ICatalogService
    {
        private readonly IRequestProvider _requestProvider;
        private readonly IFixUriService _fixUriService;
		
        private const string ApiUrlBase = "mobileshoppingapigw/api/v1/c/catalog";

        public CatalogService(IRequestProvider requestProvider, IFixUriService fixUriService)
        {
            _requestProvider = requestProvider;
            _fixUriService = fixUriService;
        }

        public async Task<ObservableCollection<CatalogItem>> FilterAsync(int catalogBrandId, int catalogTypeId)
        {
            UriBuilder builder = new UriBuilder(GlobalSetting.Instance.BaseEndpoint);
            builder.Path = $"{ApiUrlBase}/items/type/{catalogTypeId}/brand/{catalogBrandId}";
            string uri = builder.ToString();

            CatalogRoot catalog = await _requestProvider.GetAsync<CatalogRoot>(uri);

            if (catalog?.Data != null)
                return catalog?.Data.ToObservableCollection();
            else
                return new ObservableCollection<CatalogItem>();
        }

        public async Task<ObservableCollection<CatalogItem>> GetCatalogAsync()
        {
            UriBuilder builder = new UriBuilder(GlobalSetting.Instance.BaseEndpoint);
            builder.Path = $"{ApiUrlBase}/items";
            string uri = builder.ToString();

            CatalogRoot catalog = await _requestProvider.GetAsync<CatalogRoot>(uri);

            if (catalog?.Data != null)
            {
                _fixUriService.FixCatalogItemPictureUri(catalog?.Data);
                return catalog?.Data.ToObservableCollection();
            }
            else
                return new ObservableCollection<CatalogItem>();
        }

        public async Task<ObservableCollection<CatalogBrand>> GetCatalogBrandAsync()
        {
            UriBuilder builder = new UriBuilder(GlobalSetting.Instance.BaseEndpoint);
            builder.Path = $"{ApiUrlBase}/catalogbrands";
            string uri = builder.ToString();

            IEnumerable<CatalogBrand> brands = await _requestProvider.GetAsync<IEnumerable<CatalogBrand>>(uri);

            if (brands != null)
                return brands?.ToObservableCollection();
            else
                return new ObservableCollection<CatalogBrand>();
        }

        public async Task<ObservableCollection<CatalogType>> GetCatalogTypeAsync()
        {
            UriBuilder builder = new UriBuilder(GlobalSetting.Instance.BaseEndpoint);
            builder.Path = $"{ApiUrlBase}/catalogtypes";
            string uri = builder.ToString();

            IEnumerable<CatalogType> types = await _requestProvider.GetAsync<IEnumerable<CatalogType>>(uri);

            if (types != null)
                return types.ToObservableCollection();
            else
                return new ObservableCollection<CatalogType>();
        }
    }
}
