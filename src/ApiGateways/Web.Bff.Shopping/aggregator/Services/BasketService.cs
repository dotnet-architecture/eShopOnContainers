using GrpcBasket;
using Microsoft.eShopOnContainers.Web.Shopping.HttpAggregator.Models;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.eShopOnContainers.Web.Shopping.HttpAggregator.Services
{
    public class BasketService : IBasketService
    {
        private readonly Basket.BasketClient _basketClient;
        private readonly ILogger<BasketService> _logger;

        public BasketService(Basket.BasketClient basketClient, ILogger<BasketService> logger)
        {
            _basketClient = basketClient;
            _logger = logger;
        }


        public async Task<BasketData> GetById(string id)
        {
            _logger.LogDebug("grpc client created, request = {@id}", id);
            var response = await _basketClient.GetBasketByIdAsync(new BasketRequest { Id = id });
            _logger.LogDebug("grpc response {@response}", response);

            return MapToBasketData(response);
        }

        public async Task UpdateAsync(BasketData currentBasket)
        {
            _logger.LogDebug("Grpc update basket currentBasket {@currentBasket}", currentBasket);
            var request = MapToCustomerBasketRequest(currentBasket);
            _logger.LogDebug("Grpc update basket request {@request}", request);

            await _basketClient.UpdateBasketAsync(request);
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

            customerBasketRequest.Items.ToList().ForEach(item =>
            {
                if (item.Id != null)
                {
                    map.Items.Add(new BasketDataItem
                    {
                        Id = item.Id,
                        OldUnitPrice = (decimal)item.Oldunitprice,
                        PictureUrl = item.Pictureurl,
                        ProductId = item.Productid,
                        ProductName = item.Productname,
                        Quantity = item.Quantity,
                        UnitPrice = (decimal)item.Unitprice
                    });
                }
            });

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

            basketData.Items.ToList().ForEach(item =>
            {
                if (item.Id != null)
                {
                    map.Items.Add(new BasketItemResponse
                    {
                        Id = item.Id,
                        Oldunitprice = (double)item.OldUnitPrice,
                        Pictureurl = item.PictureUrl,
                        Productid = item.ProductId,
                        Productname = item.ProductName,
                        Quantity = item.Quantity,
                        Unitprice = (double)item.UnitPrice
                    });
                }
            });

            return map;
        }
    }
}
