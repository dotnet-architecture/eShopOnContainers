using Microsoft.eShopOnContainers.Web.Shopping.HttpAggregator.Config;
using Microsoft.eShopOnContainers.Web.Shopping.HttpAggregator.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using CatalogApi;
using Grpc.Net.Client;
using System;
using static CatalogApi.Catalog;
using System.Linq;
using Grpc.Core;

namespace Microsoft.eShopOnContainers.Web.Shopping.HttpAggregator.Services
{
    public class CatalogService : ICatalogService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<CatalogService> _logger;
        private readonly UrlsConfig _urls;

        public CatalogService(HttpClient httpClient, ILogger<CatalogService> logger, IOptions<UrlsConfig> config)
        {
            _httpClient = httpClient;
            _logger = logger;
            _urls = config.Value;
        }

        public async Task<CatalogItem> GetCatalogItemAsync(int id)
        {
            _httpClient.BaseAddress = new Uri(_urls.Catalog + UrlsConfig.CatalogOperations.GetItemById(id));

            var client = GrpcClient.Create<CatalogClient>(_httpClient);
            var request = new CatalogItemRequest { Id = id };
            var response = await client.GetItemByIdAsync(request);

            return MapToCatalogItemResponse(response);
        }

        public async Task<IEnumerable<CatalogItem>> GetCatalogItemsAsync(IEnumerable<int> ids)
        {

            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2Support", true);

            using (var httpClientHandler = new HttpClientHandler())
            {
                httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
                using (var httpClient = new HttpClient(httpClientHandler))
                {
                    httpClient.BaseAddress = new Uri(_urls.GrpcCatalog);

                    _logger.LogInformation("Creating grpc client for CatalogClient {@httpClient.BaseAddress}, {@httpClient} ", httpClient.BaseAddress, httpClient);

                    try
                    {
                        var client = GrpcClient.Create<CatalogClient>(httpClient);
                        var request = new CatalogItemsRequest { Ids = string.Join(",", ids), PageIndex = 1, PageSize = 10 };
                        _logger.LogInformation("grpc client created, request = {@request}", request);
                        var response = await client.GetItemsByIdsAsync(request);
                        _logger.LogInformation("grpc response {@response}", response);
                        return response.Data.Select(this.MapToCatalogItemResponse);
                    }
                    catch (RpcException e)
                    {
                        _logger.LogError($"Error calling via grpc: {e.Status} - {e.Message}");
                    }
                }
            }

            return null;
        }

        private CatalogItem MapToCatalogItemResponse(CatalogItemResponse catalogItemResponse)
        {
            return new CatalogItem
            {
                Id = catalogItemResponse.Id,
                Name = catalogItemResponse.Name,
                PictureUri = catalogItemResponse.PictureUri,
                Price = (decimal)catalogItemResponse.Price
            };
        }
    }
}
