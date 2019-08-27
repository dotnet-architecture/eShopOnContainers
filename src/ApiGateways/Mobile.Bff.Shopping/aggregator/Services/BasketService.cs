using Grpc.Net.Client;
using Microsoft.eShopOnContainers.Mobile.Shopping.HttpAggregator.Config;
using Microsoft.eShopOnContainers.Mobile.Shopping.HttpAggregator.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using GrpcBasket;
using Grpc.Core;

namespace Microsoft.eShopOnContainers.Mobile.Shopping.HttpAggregator.Services
{
    public class BasketService : IBasketService
    {
        private readonly HttpClient _httpClient;
        private readonly UrlsConfig _urls;
        private readonly ILogger<BasketService> _logger;

        public BasketService(HttpClient httpClient, IOptions<UrlsConfig> config, ILogger<BasketService> logger)
        {
            _httpClient = httpClient;
            _urls = config.Value;
            _logger = logger;
        }

        public async Task<BasketData> GetById(string id)
        {

            _logger.LogInformation("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ GetById @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");

            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2Support", true);

            using (var httpClientHandler = new HttpClientHandler())
            {
                httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
                using (var httpClient = new HttpClient(httpClientHandler))
                {
                    _logger.LogInformation("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ Http2UnencryptedSupport disable @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");

                    httpClient.BaseAddress = new Uri("http://localhost:5580");

                    _logger.LogInformation("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ {httpClient.BaseAddress} @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ " + httpClient.BaseAddress, httpClient.BaseAddress);

                    var client = GrpcClient.Create<Basket.BasketClient>(httpClient);

                    _logger.LogInformation("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ client create @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");

                    try
                    {

                        var response = await client.GetBasketByIdAsync(new BasketRequest { Id = id });
                        _logger.LogInformation("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ call grpc server @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");

                        _logger.LogInformation("############## DATA: {@a}", response.Buyerid);
                        _logger.LogInformation("############## DATA:response {@response}", response);

                        return MapToBasketData(response);
                    }
                    catch (RpcException e)
                    {
                        _logger.LogError($"Error calling via grpc: {e.Status} - {e.Message}");
                    }
                }
            }

            return null; // temp
            // var data = await _apiClient.GetStringAsync(_urls.Basket +  UrlsConfig.BasketOperations.GetItemById(id));
            // var basket = !string.IsNullOrEmpty(data) ? JsonConvert.DeserializeObject<BasketData>(data) : null;

            // return basket;
        }

        public async Task UpdateAsync(BasketData currentBasket)
        {
            _httpClient.BaseAddress = new Uri(_urls.Basket + UrlsConfig.BasketOperations.UpdateBasket());

            var client = GrpcClient.Create<Basket.BasketClient>(_httpClient);
            var request = MapToCustomerBasketRequest(currentBasket);

            await client.UpdateBasketAsync(request);
        }

        private BasketData MapToBasketData(CustomerBasketResponse customerBasketRequest)
        {
            if (customerBasketRequest == null)
            {
                return null;
            }

            var map = new BasketData
            {
                BuyerId = customerBasketRequest.Buyerid
            };

            customerBasketRequest.Items.ToList().ForEach(item => map.Items.Add(new BasketDataItem
            {
                Id = item.Id,
                OldUnitPrice = (decimal)item.Oldunitprice,
                PictureUrl = item.Pictureurl,
                ProductId = item.Productid,
                ProductName = item.Productname,
                Quantity = item.Quantity,
                UnitPrice = (decimal)item.Unitprice
            }));

            return map;
        }

        private CustomerBasketRequest MapToCustomerBasketRequest(BasketData basketData)
        {
            if (basketData == null)
            {
                return null;
            }

            var map = new CustomerBasketRequest
            {
                Buyerid = basketData.BuyerId
            };

            basketData.Items.ToList().ForEach(item => map.Items.Add(new BasketItemResponse
            {
                Id = item.Id,
                Oldunitprice = (double)item.OldUnitPrice,
                Pictureurl = item.PictureUrl,
                Productid = item.ProductId,
                Productname = item.ProductName,
                Quantity = item.Quantity,
                Unitprice = (double)item.UnitPrice
            }));

            return map;
        }
    }
}
