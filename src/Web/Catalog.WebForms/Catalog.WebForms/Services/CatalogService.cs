using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using eShopOnContainers.Core.Models.Catalog;
using eShopOnContainers.Core.Services.RequestProvider;
using eShopOnContainers.Core.Extensions;
using System.Collections.Generic;

// from https://github.com/dotnet/eShopOnContainers/blob/vs2017/src/Mobile/eShopOnContainers/eShopOnContainers.Core/Services/Catalog/CatalogService.cs
// TODO: DRY this stuff.
namespace eShopOnContainers.Core.Services.Catalog
{
    public class CatalogService : ICatalogService
    {
        private readonly IRequestProvider _requestProvider;

        public CatalogService(IRequestProvider requestProvider)
        {
            _requestProvider = requestProvider;
        }

        public async Task<ObservableCollection<CatalogItem>> FilterAsync(int catalogBrandId, int catalogTypeId)
        {
            try
            {
                var baseURl = System.Configuration.ConfigurationManager.AppSettings["CatalogURL"];
                UriBuilder builder = new UriBuilder(baseURl);

                builder.Path = string.Format("api/v1/catalog/items/type/{0}/brand/{1}", catalogTypeId, catalogBrandId);

                string uri = builder.ToString();

                CatalogRoot catalog =
                        await _requestProvider.GetAsync<CatalogRoot>(uri);

                if (catalog?.Data != null)
                    return catalog?.Data.ToObservableCollection();
                else
                    return new ObservableCollection<CatalogItem>();
            }
            catch
            {
                return new ObservableCollection<CatalogItem>();
            }
        }

        public async Task<ObservableCollection<CatalogItem>> GetCatalogAsync()
        {
            try
            {
                var baseURl = System.Configuration.ConfigurationManager.AppSettings["CatalogURL"];
                UriBuilder builder = new UriBuilder(baseURl);

                builder.Path = "api/v1/catalog/items";

                string uri = builder.ToString();

                CatalogRoot catalog =
                    await _requestProvider.GetAsync<CatalogRoot>(uri);

                if (catalog?.Data != null)
                {
                    // TODO: ServicesHelper.FixCatalogItemPictureUri(catalog?.Data);

                    return catalog?.Data.ToObservableCollection();
                }
                else
                    return new ObservableCollection<CatalogItem>();
            }
            catch
            {
                return new ObservableCollection<CatalogItem>();
            }
        }

        public Task<CatalogItem> GetCatalogItemAsync(string id)
        {
            var baseURl = System.Configuration.ConfigurationManager.AppSettings["CatalogURL"];
            UriBuilder builder = new UriBuilder(baseURl);

            builder.Path = $"api/v1/catalog/items/{id}";

            string uri = builder.ToString();

            return _requestProvider.GetAsync<CatalogItem>(uri);
        }

        public async Task<ObservableCollection<CatalogBrand>> GetCatalogBrandAsync()
        {
            try
            {
                var baseURl = System.Configuration.ConfigurationManager.AppSettings["CatalogURL"];
                UriBuilder builder = new UriBuilder(baseURl);

                builder.Path = "api/v1/catalog/catalogbrands";

                string uri = builder.ToString();

                IEnumerable<CatalogBrand> brands =
                       await _requestProvider.GetAsync<IEnumerable<CatalogBrand>>(uri);

                if (brands != null)
                    return brands?.ToObservableCollection();
                else
                    return new ObservableCollection<CatalogBrand>();
            }
            catch
            {
                return new ObservableCollection<CatalogBrand>();
            }
        }

        public async Task<ObservableCollection<CatalogType>> GetCatalogTypeAsync()
        {
            try
            {
                var baseURl = System.Configuration.ConfigurationManager.AppSettings["CatalogURL"];
                UriBuilder builder = new UriBuilder(baseURl);

                builder.Path = "api/v1/catalog/catalogtypes";

                string uri = builder.ToString();

                IEnumerable<CatalogType> types =
                       await _requestProvider.GetAsync<IEnumerable<CatalogType>>(uri);

                if (types != null)
                    return types.ToObservableCollection();
                else
                    return new ObservableCollection<CatalogType>();
            }
            catch
            {
                return new ObservableCollection<CatalogType>();
            }
        }

        public Task DeleteCatalogItemAsync(string catalogItemId)
        {
            var baseURl = System.Configuration.ConfigurationManager.AppSettings["CatalogURL"];
            UriBuilder builder = new UriBuilder(baseURl);

            builder.Path = $"api/v1/catalog/{catalogItemId}";

            string uri = builder.ToString();

            return _requestProvider.DeleteAsync(uri);
        }

        public Task<CatalogItem> UpdateCatalogItemAsync(CatalogItem item)
        {
            var baseURl = System.Configuration.ConfigurationManager.AppSettings["CatalogURL"];
            UriBuilder builder = new UriBuilder(baseURl);

            builder.Path = "api/v1/catalog/edit";

            string uri = builder.ToString();

            return _requestProvider.PostAsync(uri, item);
        }

        public Task<CatalogItem> CreateCatalogItemAsync(CatalogItem item)
        {
            var baseURl = System.Configuration.ConfigurationManager.AppSettings["CatalogURL"];
            UriBuilder builder = new UriBuilder(baseURl);

            builder.Path = "api/v1/catalog/create";

            string uri = builder.ToString();

            return _requestProvider.PostAsync(uri, item);

        }
    }
}
