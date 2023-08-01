using Microsoft.eShopOnContainers.Services.Ordering.Domain.Seedwork;

namespace Microsoft.eShopOnContainers.Services.Ordering.API.Application.Commands
{
    public class CompleteOrderCommandHandler : IRequestHandler<CompleteOrderCommand, bool>
    {
        private readonly IOrderRepository _orderRepository;

        public CompleteOrderCommandHandler(IOrderRepository orderRepository)
        {
            //order repoyu DI ettik
            this._orderRepository = orderRepository;
        }
        //Mediatr Handler dan implement ediyoruz
        public async Task<bool> Handle(CompleteOrderCommand request, CancellationToken cancellationToken)
        {
            //bakalım gelinen order no mevcut mu
            var shippedOrder = await _orderRepository.GetAsync(request.OrderNumber);
            if (shippedOrder != null)
            {
                shippedOrder.SetCompletedStatus();
                return await _orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
            }
            else
                return false;
        }
    }
}
