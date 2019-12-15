namespace Microsoft.eShopOnContainers.Services.Ordering.API.Application.Commands
{
    using Domain.AggregatesModel.OrderAggregate;
    using global::Ordering.API.Application.Models;
    using MediatR;
    using Microsoft.eShopOnContainers.Services.Ordering.API.Infrastructure.Services;
    using Microsoft.eShopOnContainers.Services.Ordering.Infrastructure.Idempotency;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using static Microsoft.eShopOnContainers.Services.Ordering.API.Application.Commands.CreateOrderCommand;

    // Regular CommandHandler
    public class CreateOrderDraftCommandHandler
        : IRequestHandler<CreateOrderDraftCommand, OrderDraftDTO>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IIdentityService _identityService;
        private readonly IMediator _mediator;

        // Using DI to inject infrastructure persistence Repositories
        public CreateOrderDraftCommandHandler(IMediator mediator,  IIdentityService identityService)
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


    public class OrderDraftDTO
    {
        public IEnumerable<OrderItemDTO> OrderItems { get; set; }
        public decimal Total { get; set; }

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




}
