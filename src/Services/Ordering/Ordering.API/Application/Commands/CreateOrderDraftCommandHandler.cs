﻿namespace Microsoft.eShopOnContainers.Services.Ordering.API.Application.Commands;

using static Microsoft.eShopOnContainers.Services.Ordering.API.Application.Commands.CreateOrderCommand;
using Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel.OrderAggregate;

// Regular CommandHandler
public class CreateOrderDraftCommandHandler
    : IRequestHandler<CreateOrderDraftCommand, OrderDraftDTO>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IIdentityService _identityService;
    private readonly IMediator _mediator;

    // Using DI to inject infrastructure persistence Repositories
    public CreateOrderDraftCommandHandler(IMediator mediator, IIdentityService identityService)
    {
        _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public Task<OrderDraftDTO> Handle(CreateOrderDraftCommand message, CancellationToken cancellationToken)
    {

        var order = Order.NewDraft();
        var orderItems = message.Items.Select(i => i.ToOrderItemDTO());
        foreach (var item in orderItems)
        {
            order.AddOrderItem(item.ProductId, item.ProductName, item.UnitPrice, item.Discount, item.PictureUrl, item.Units);
        }

        return Task.FromResult(OrderDraftDTO.FromOrder(order));
    }
}


public record OrderDraftDTO
{
    public IEnumerable<OrderItemDTO> OrderItems { get; init; }
    public decimal Total { get; init; }

    public static OrderDraftDTO FromOrder(Order order)
    {
        return new OrderDraftDTO()
        {
            OrderItems = order.OrderItems.Select(oi => new OrderItemDTO
            {
                Discount = oi.GetCurrentDiscount(),
                ProductId = oi.ProductId,
                UnitPrice = oi.GetUnitPrice(),
                PictureUrl = oi.GetPictureUri(),
                Units = oi.GetUnits(),
                ProductName = oi.GetOrderItemProductName()
            }),
            Total = order.GetTotal()
        };
    }

}