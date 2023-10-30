using Newtonsoft.Json;
using System.Text;

namespace Microsoft.eShopOnContainers.Web.Shopping.HttpAggregator.Services;

public class OrderingService : IOrderingService
{
    private readonly GrpcOrdering.OrderingGrpc.OrderingGrpcClient _orderingGrpcClient;
    private readonly ILogger<OrderingService> _logger;
    private readonly HttpClient _httpClient;

    public OrderingService(GrpcOrdering.OrderingGrpc.OrderingGrpcClient orderingGrpcClient, ILogger<OrderingService> logger, HttpClient httpClient)
    {
        _orderingGrpcClient = orderingGrpcClient;
        _logger = logger;
        _httpClient = httpClient;
    }

    public async Task<OrderData> GetOrderDraftAsync(BasketData basketData)
    {
        _logger.LogDebug(" grpc client created, basketData={@basketData}", basketData);

        var command = MapToOrderDraftCommand(basketData);
        var response = await _orderingGrpcClient.CreateOrderDraftFromBasketDataAsync(command);
        _logger.LogDebug(" grpc response: {@response}", response);

        return MapToResponse(response, basketData);
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

    private GrpcOrdering.CreateOrderDraftCommand MapToOrderDraftCommand(BasketData basketData)
    {
        var command = new GrpcOrdering.CreateOrderDraftCommand
        {
            BuyerId = basketData.BuyerId,
        };

        basketData.Items.ForEach(i => command.Items.Add(new GrpcOrdering.BasketItem
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
    /// <summary>
    /// CompleteOrderAsync is the endpoint that will be called to indicate that the order is complete.
    /// </summary>
    /// <param name="orderId">Order number</param>
    /// <returns></returns>
    public async Task<CompleteData> CompleteOrderAsync(string orderId)
    {
        // TODO  Grpc OrderingGrpc bağlantı servisinde hata alınmaktadır.Bu nedenle httpclient sınıfı kullanılmıştır.
        #region OrderingGrpc
        CompleteData completeData = new CompleteData();
        _logger.LogDebug("CompleteOrderAsync method called with orderId={@orderId}", orderId);

        var request = new GrpcOrdering.CompleteOrderCommand
        {
            OrderId = orderId
        };
        var response = await _orderingGrpcClient.CompleteOrderAsync(request);

        _logger.LogDebug("gRPC CompleteOrder response: {@response}", response);
        completeData.CompleteStatus = response.CompleteStatus;
        return completeData;
        #endregion
    }

}
