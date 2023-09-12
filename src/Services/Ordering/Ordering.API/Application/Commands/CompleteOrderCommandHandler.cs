namespace Microsoft.eShopOnContainers.Services.Ordering.API.Application.Commands
{
    public class CompleteOrderCommandHandler : IRequestHandler<CompleteOrderCommand, bool>
    {
        private readonly IOrderRepository _orderRepository;

        public CompleteOrderCommandHandler(IOrderRepository orderRepository)
        {
            this._orderRepository = orderRepository;
        }

        //OrderNo mevcut mu kontrolü yapılır
        public async Task<bool> Handle(CompleteOrderCommand request, CancellationToken cancellationToken)
        {
            var checkedOrder = await _orderRepository.GetAsync(request.OrderNo);
            if (checkedOrder != null)
            {
                checkedOrder.SetCompletedStatus();
                return await _orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
            }
            else
                return false;
        }
    }
}
