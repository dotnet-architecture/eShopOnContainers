using MediatR;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
using Microsoft.eShopOnContainers.Services.Ordering.API.Application.Commands;
using Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel.OrderAggregate;
using Microsoft.eShopOnContainers.Services.Ordering.Infrastructure;
using Microsoft.eShopOnContainers.Services.Ordering.Infrastructure.Idempotency;
using Ordering.API.Application.Commands;
using Ordering.API.Application.IntegrationCommands.Commands;
using Ordering.API.Application.IntegrationEvents;
using Ordering.Domain.Exceptions;
using System.Threading.Tasks;

namespace Ordering.API.Application.Sagas
{
    /// <summary>
    /// Saga for handling the place order process
    /// and which is started once the basket.api has 
    /// successfully processed the items ordered.
    /// Saga provides a period of grace to give the customer
    /// the opportunity to cancel the order before proceeding
    /// with the validations.
    /// </summary>
    public class OrderProcessSaga : OrderSaga,
        IIntegrationEventHandler<ConfirmGracePeriodCommandMsg>,
        IAsyncRequestHandler<CancelOrderCommand, bool>,
        IAsyncRequestHandler<ShipOrderCommand, bool>
    {

        public OrderProcessSaga(
            OrderingContext orderingContext)
            : base(orderingContext)
        {
        }        

        /// <summary>
        /// Command handler which confirms that the grace period
        /// has been completed and order has not been cancelled.
        /// If so, the process continues for validation. 
        /// </summary>
        /// <param name="event">
        /// Integration command message which is sent by a saga 
        /// scheduler which provides the sagas that its grace 
        /// period has completed.
        /// </param>
        /// <returns></returns>
        public async Task Handle(ConfirmGracePeriodCommandMsg @event)
        {
            var orderSaga = FindSagaById(@event.OrderId);
            CheckValidSagaId(orderSaga);

            if (orderSaga.OrderStatus != OrderStatus.Cancelled)
            {
                orderSaga.SetAwaitingValidationStatus();

                await SaveChangesAsync();
            }
        }
        
        /// <summary>
        /// Handler which processes the command when
        /// customer executes cancel order from app
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public async Task<bool> Handle(CancelOrderCommand command)
        {
            var result = false;
            var orderSaga = FindSagaById(command.OrderNumber);
            CheckValidSagaId(orderSaga);

            // Not possible to cancel order when 
            // it has already been shipped
            if (orderSaga.GetOrderStatusId() != OrderStatus.Cancelled.Id
                || orderSaga.GetOrderStatusId() != OrderStatus.Shipped.Id)
            {
                orderSaga.SetCancelStatus();
                result = await SaveChangesAsync();
            }
            return result;
        }

        /// <summary>
        /// Handler which processes the command when
        /// administrator executes ship order from app
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public async Task<bool> Handle(ShipOrderCommand command)
        {
            var result = false;
            var orderSaga = FindSagaById(command.OrderNumber);
            CheckValidSagaId(orderSaga);

            // Only ship order when 
            // its status is paid
            if (orderSaga.GetOrderStatusId() == OrderStatus.Paid.Id)
            {
                orderSaga.SetShippedStatus();
                result = await SaveChangesAsync();
            }
            return result;
        }

        private void CheckValidSagaId(Order orderSaga)
        {
            if (orderSaga is null)
            {
                throw new OrderingDomainException("Not able to process order saga event. Reason: no valid orderId");
            }
        }

        #region CommandHandlerIdentifiers

        public class CancelOrderCommandIdentifiedHandler : IdentifierCommandHandler<CancelOrderCommand, bool>
        {
            public CancelOrderCommandIdentifiedHandler(IMediator mediator, IRequestManager requestManager) : base(mediator, requestManager)
            {
            }

            protected override bool CreateResultForDuplicateRequest()
            {
                return true;                // Ignore duplicate requests for processing order.
            }
        }

        public class ShipOrderCommandIdentifiedHandler : IdentifierCommandHandler<ShipOrderCommand, bool>
        {
            public ShipOrderCommandIdentifiedHandler(IMediator mediator, IRequestManager requestManager) : base(mediator, requestManager)
            {
            }

            protected override bool CreateResultForDuplicateRequest()
            {
                return true;                // Ignore duplicate requests for processing order.
            }
        }

        #endregion
    }
}
