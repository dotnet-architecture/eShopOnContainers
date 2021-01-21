using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.eShopOnContainers.Services.Ordering.API.Application.Commands;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Extensions;
using MediatR;
using Grpc.Core;
using AppCommand = Microsoft.eShopOnContainers.Services.Ordering.API.Application.Commands;
using ApiModels = Ordering.API.Application.Models;
using Google.Protobuf.Collections;
using System.Collections.Generic;

namespace GrpcOrdering
{
    public class OrderingService : OrderingGrpc.OrderingGrpcBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<OrderingService> _logger;

        public OrderingService(IMediator mediator, ILogger<OrderingService> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public override async Task<OrderDraftDTO> CreateOrderDraftFromBasketData(CreateOrderDraftCommand createOrderDraftCommand, ServerCallContext context)
        {
            _logger.LogInformation("Begin grpc call from method {Method} for ordering get order draft {CreateOrderDraftCommand}", context.Method, createOrderDraftCommand);
            _logger.LogTrace(
                "----- Sending command: {CommandName} - {IdProperty}: {CommandId} ({@Command})",
                createOrderDraftCommand.GetGenericTypeName(),
                nameof(createOrderDraftCommand.BuyerId),
                createOrderDraftCommand.BuyerId,
                createOrderDraftCommand);

            var command = new AppCommand.CreateOrderDraftCommand(
                            createOrderDraftCommand.BuyerId,
                            this.MapBasketItems(createOrderDraftCommand.Items));


            var data = await _mediator.Send(command);

            if (data != null)
            {
                context.Status = new Status(StatusCode.OK, $" ordering get order draft {createOrderDraftCommand} do exist");

                return this.MapResponse(data);
            }
            else
            {
                context.Status = new Status(StatusCode.NotFound, $" ordering get order draft {createOrderDraftCommand} do not exist");
            }

            return new OrderDraftDTO();
        }

        public OrderDraftDTO MapResponse(AppCommand.OrderDraftDTO order)
        {
            var result = new OrderDraftDTO()
            {
                Total = (double)order.Total,
            };

            order.OrderItems.ToList().ForEach(i => result.OrderItems.Add(new OrderItemDTO()
            {
                Discount = (double)i.Discount,
                PictureUrl = i.PictureUrl,
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                UnitPrice = (double)i.UnitPrice,
                Units = i.Units,
            }));

            return result;
        }

        public IEnumerable<ApiModels.BasketItem> MapBasketItems(RepeatedField<BasketItem> items)
        {
            return items.Select(x => new ApiModels.BasketItem()
            {
                Id = x.Id,
                ProductId = x.ProductId,
                ProductName = x.ProductName,
                UnitPrice = (decimal)x.UnitPrice,
                OldUnitPrice = (decimal)x.OldUnitPrice,
                Quantity = x.Quantity,
                PictureUrl = x.PictureUrl,
            });
        }
    }
}
