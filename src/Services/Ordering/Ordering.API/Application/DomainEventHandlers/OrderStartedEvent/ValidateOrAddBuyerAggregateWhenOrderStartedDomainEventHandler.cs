using MediatR;
using Microsoft.eShopOnContainers.Services.Ordering.API.Infrastructure.Services;
using Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel.BuyerAggregate;
using Microsoft.Extensions.Logging;
using Ordering.Domain.Events;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.eShopOnContainers.Services.Ordering.Infrastructure;

namespace Ordering.API.Application.DomainEventHandlers.OrderStartedEvent
{
    public class ValidateOrAddBuyerAggregateWhenOrderStartedDomainEventHandler 
                        : INotificationHandler<DomainEventNotification<OrderStartedDomainEvent>>
    {
        private readonly ILoggerFactory _logger;
        private readonly IBuyerRepository _buyerRepository;
        private readonly IIdentityService _identityService;

        public ValidateOrAddBuyerAggregateWhenOrderStartedDomainEventHandler(ILoggerFactory logger, IBuyerRepository buyerRepository, IIdentityService identityService)
        {
            _buyerRepository = buyerRepository ?? throw new ArgumentNullException(nameof(buyerRepository));
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(DomainEventNotification<OrderStartedDomainEvent> orderStartedEventNotification, CancellationToken cancellationToken)
        {
            var cardTypeId = (orderStartedEventNotification.Event.CardTypeId != 0) ? orderStartedEventNotification.Event.CardTypeId : 1;
            var buyer = await _buyerRepository.FindAsync(orderStartedEventNotification.Event.UserId);
            bool buyerOriginallyExisted = (buyer == null) ? false : true;

            if (!buyerOriginallyExisted)
            {                
                buyer = new Buyer(orderStartedEventNotification.Event.UserId);
            }

            buyer.VerifyOrAddPaymentMethod(cardTypeId,
                                           $"Payment Method on {DateTime.UtcNow}",
                                           orderStartedEventNotification.Event.CardNumber,
                                           orderStartedEventNotification.Event.CardSecurityNumber,
                                           orderStartedEventNotification.Event.CardHolderName,
                                           orderStartedEventNotification.Event.CardExpiration,
                                           orderStartedEventNotification.Event.Order.Id);

            var buyerUpdated = buyerOriginallyExisted ? _buyerRepository.Update(buyer) : _buyerRepository.Add(buyer);

            await _buyerRepository.UnitOfWork
                .SaveEntitiesAsync();

            _logger.CreateLogger(nameof(ValidateOrAddBuyerAggregateWhenOrderStartedDomainEventHandler)).LogTrace($"Buyer {buyerUpdated.Id} and related payment method were validated or updated for orderId: {orderStartedEventNotification.Event.Order.Id}.");
        }
    }
}
