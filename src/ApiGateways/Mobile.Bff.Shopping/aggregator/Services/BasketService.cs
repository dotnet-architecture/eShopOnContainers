using grpc;
using Grpc.Net.Client;
using Microsoft.eShopOnContainers.Mobile.Shopping.HttpAggregator.Config;
using Microsoft.eShopOnContainers.Mobile.Shopping.HttpAggregator.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using static grpc.Basket;

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

            _logger.LogInformation("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ Http2UnencryptedSupport disable @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");

            _httpClient.BaseAddress = new Uri("http://localhost:5001");

            _logger.LogInformation("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ {_httpClient.BaseAddress} @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@", _httpClient.BaseAddress);

            var client = GrpcClient.Create<BasketClient>(_httpClient);

            _logger.LogInformation("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ client create @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");

            var response = await client.GetBasketByIdAsync(new BasketRequest { Id = id });

            _logger.LogInformation("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ call grpc server @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");

            _logger.LogInformation("############## DATA: {@a}", response.Buyerid);

            //if (streaming.IsCompleted)
            //{
            //    _logger.LogInformation("############## DATA: {@a}", streaming.GetResult());
            //}
            //var streaming = client.GetBasketById(new BasketRequest { Id = id });


            //var status = streaming.GetStatus();

            //if (status.StatusCode == Grpc.Core.StatusCode.OK)
            //{
            //    return null;
            //}

            return null;
            //return MapToBasketData(response.ResponseStream);
        }

        public async Task UpdateAsync(BasketData currentBasket)
        {
            _httpClient.BaseAddress = new Uri(_urls.Basket + UrlsConfig.BasketOperations.UpdateBasket());

            var client = GrpcClient.Create<BasketClient>(_httpClient);
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
