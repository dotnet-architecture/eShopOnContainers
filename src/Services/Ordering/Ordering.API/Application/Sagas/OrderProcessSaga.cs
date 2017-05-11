using Autofac.Features.OwnedInstances;
using MediatR;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
using Microsoft.eShopOnContainers.Services.Ordering.API.Application.Commands;
using Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel.OrderAggregate;
using Microsoft.eShopOnContainers.Services.Ordering.Infrastructure;
using Microsoft.eShopOnContainers.Services.Ordering.Infrastructure.Idempotency;
using Ordering.API.Application.Commands;
using Ordering.API.Application.IntegrationCommands.Commands;
using Ordering.API.Application.IntegrationEvents.Events;
using Ordering.Domain.Exceptions;
using System;
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
    public class OrderProcessSaga : Saga<Order>,
        IIntegrationEventHandler<UserCheckoutAcceptedIntegrationEvent>,
        IIntegrationEventHandler<ConfirmGracePeriodCommandMsg>,
        IAsyncRequestHandler<CancelOrderCommand, bool>
    {
        private readonly IMediator _mediator;
        private readonly Func<Owned<OrderingContext>> _dbContextFactory;

        public OrderProcessSaga(
            Func<Owned<OrderingContext>> dbContextFactory, OrderingContext orderingContext,
            IMediator mediator)
            : base(orderingContext)
        {
            _dbContextFactory = dbContextFactory;
            _mediator = mediator;
        }

        /// <summary>
        /// Command handler which starts the create order process
        /// and initializes the saga
        /// </summary>
        /// <param name="command">
        /// Integration command message which is sent by the
        /// basket.api once it has successfully process the 
        /// order items.
        /// </param>
        /// <returns></returns>
        public async Task Handle(UserCheckoutAcceptedIntegrationEvent command)
        {

            var commanda = command; 

            // TODO: This handler should change to Integration command handler type once command bus is implemented

            // TODO: Send createOrder Command

            // TODO: Set saga timeout            
        }

        /// <summary>
        /// Command handler which confirms that the grace period
        /// has been completed and order has not been cancelled.
        /// If so, the process continues for validation. 
        /// </summary>
        /// <param name="command">
        /// Integration command message which is sent by a saga 
        /// scheduler which provides the sagas that its grace 
        /// period has completed.
        /// </param>
        /// <returns></returns>
        public async Task Handle(ConfirmGracePeriodCommandMsg command)
        {
            var orderSaga = FindSagaById(command.OrderNumber);
            CheckValidSagaId(orderSaga);

            // TODO: This handler should change to Integration command handler type once command bus is implemented

            // TODO: If order status is not cancelled, change state to awaitingValidation and 
            // send ConfirmOrderStockCommandMsg to Inventory api           
        }
        

        /// <summary>
        /// Handler which processes the command when
        /// customer executes cancel order from app
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public async Task<bool> Handle(CancelOrderCommand command)
        {
            var orderSaga = FindSagaById(command.OrderNumber);
            CheckValidSagaId(orderSaga);

            // Set order status tu cancelled

            return true;
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

        #endregion
    }
}
