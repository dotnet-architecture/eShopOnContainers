using MediatR;
using Microsoft.eShopOnContainers.Services.Ordering.API.Application.Commands;
using Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel.OrderAggregate;
using Microsoft.eShopOnContainers.Services.Ordering.Infrastructure.Idempotency;
using System.Threading;
using System.Threading.Tasks;

namespace Ordering.API.Application.Commands
{
    // Regular CommandHandler
    public class ShipOrderCommandHandler : IRequestHandler<ShipOrderCommand, bool>
    {        
        private readonly IOrderRepository _orderRepository;

        public ShipOrderCommandHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        /// <summary>
        /// Handler which processes the command when
        /// administrator executes ship order from app
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public async Task<bool> Handle(ShipOrderCommand command, CancellationToken cancellationToken)
        {
            var orderToUpdate = await _orderRepository.GetAsync(command.OrderNumber);
            if(orderToUpdate == null)
            {
                return false;
            }

            orderToUpdate.SetShippedStatus();
            return await _orderRepository.UnitOfWork.SaveEntitiesAsync();
        }
    }


    // Use for Idempotency in Command process
    public class ShipOrderExistingCommand : ExistingCommandResponse<ShipOrderCommand, bool>
    {
        protected override Task<bool> CreateResponseForExistingCommand(ShipOrderCommand command)
        {
            // Ignore duplicate requests for creating order.
            return Task.FromResult(true);
        }
    }
}
