namespace Microsoft.eShopOnContainers.Services.Ordering.API.Application.Commands
{
    public class CompleteOrderCommandHandler : IRequestHandler<CompleteOrderCommand, CompleteOrderDTO>
    {
        private readonly IOrderRepository _orderRepository; // Varsayılan bir repo

        private readonly IMediator _mediator; // Integration event'leri fırlatmak için

        public CompleteOrderCommandHandler(IOrderRepository orderRepository, IMediator mediator)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<CompleteOrderDTO> Handle(CompleteOrderCommand request, CancellationToken cancellationToken)
        {

            //var order = await _orderRepository.GetOrderAsync(request.OrderId);
            CompleteOrderDTO completeStatus = new CompleteOrderDTO();
            var order = await _orderRepository.GetAsync(request.OrderId);
            if (order == null)
            {
                completeStatus.CompleteStatus = "Incompleted";
                return completeStatus;
            }
            order.CompleteOrder(); // The status of the order was set to "Complete".

            _orderRepository.Update(order);
            await _orderRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            await _mediator.Publish(new OrderCompletedIntegrationEvent(order.Id)); //When the process is completed, an integration event is thrown.
            completeStatus.CompleteStatus = "Completed";
            return completeStatus;
        }
    }
    public class CompleteOrderDTO
    {
        public string CompleteStatus { get; set; }

    }
}
