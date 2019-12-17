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
            return await GrpcCallerService.CallService(_urls.GrpcBasket, async channel =>
            {
                
                var client = new Basket.BasketClient(channel);
                _logger.LogDebug("grpc client created, request = {@id}", id);
                var response = await client.GetBasketByIdAsync(new BasketRequest { Id = id });
                _logger.LogDebug("grpc response {@response}", response);

                return MapToBasketData(response);
            });
        }

        public async Task UpdateAsync(BasketData currentBasket)
        {
            await GrpcCallerService.CallService(_urls.GrpcBasket, async httpClient =>
            {
                var channel = GrpcChannel.ForAddress(_urls.GrpcBasket);
                var client = new Basket.BasketClient(channel);
                _logger.LogDebug("Grpc update basket currentBasket {@currentBasket}", currentBasket);
                var request = MapToCustomerBasketRequest(currentBasket);
                _logger.LogDebug("Grpc update basket request {@request}", request);

                return await client.UpdateBasketAsync(request);
            });
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
