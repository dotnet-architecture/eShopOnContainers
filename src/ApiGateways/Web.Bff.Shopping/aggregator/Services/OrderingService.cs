using Grpc.Net.Client;
using Microsoft.eShopOnContainers.Web.Shopping.HttpAggregator.Config;
using Microsoft.eShopOnContainers.Web.Shopping.HttpAggregator.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using GrpcOrdering;
using Grpc.Core;

namespace Microsoft.eShopOnContainers.Web.Shopping.HttpAggregator.Services
{
    public class OrderingService : IOrderingService
    {
        private readonly UrlsConfig _urls;
        private readonly ILogger<OrderingService> _logger;
        public readonly HttpClient _httpClient;

        public OrderingService(HttpClient httpClient, IOptions<UrlsConfig> config, ILogger<OrderingService> logger)
        {
            _urls = config.Value;
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<OrderData> GetOrderDraftAsync(BasketData basketData)
        {
            return await GrpcCallerService.CallService(_urls.GrpcOrdering, async channel =>
            {
                var client = new OrderingGrpc.OrderingGrpcClient(channel);
                _logger.LogDebug(" grpc client created, basketData={@basketData}", basketData);

                var command = MapToOrderDraftCommand(basketData);
                var response = await client.CreateOrderDraftFromBasketDataAsync(command);
                _logger.LogDebug(" grpc response: {@response}", response);

                return MapToResponse(response, basketData);
            });
        }

        private OrderData MapToResponse(GrpcOrdering.OrderDraftDTO orderDraft, BasketData basketData)
        {
            if (orderDraft == null)
            {
                return null;
            }

            var data = new OrderData
            {
                Buyer = basketData.BuyerId,
                Total = (decimal)orderDraft.Total,
            };

            orderDraft.OrderItems.ToList().ForEach(o => data.OrderItems.Add(new OrderItemData
            {
                Discount = (decimal)o.Discount,
                PictureUrl = o.PictureUrl,
                ProductId = o.ProductId,
                ProductName = o.ProductName,
                UnitPrice = (decimal)o.UnitPrice,
                Units = o.Units,
            }));

            return data;
        }

        private CreateOrderDraftCommand MapToOrderDraftCommand(BasketData basketData)
        {
            var command = new CreateOrderDraftCommand
            {
                BuyerId = basketData.BuyerId,
            };

            basketData.Items.ForEach(i => command.Items.Add(new BasketItem
            {
                Id = i.Id,
                OldUnitPrice = (double)i.OldUnitPrice,
                PictureUrl = i.PictureUrl,
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                Quantity = i.Quantity,
                UnitPrice = (double)i.UnitPrice,
            }));

            return command;
        }

    }
}
