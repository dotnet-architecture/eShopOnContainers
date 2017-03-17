using MediatR;
using Microsoft.eShopOnContainers.Services.Ordering.API.Infrastructure.Services;
using Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel.BuyerAggregate;
using Microsoft.Extensions.Logging;
using Ordering.Domain.Events;
using System;
using System.Threading.Tasks;

namespace Ordering.API.Application.DomainEventHandlers
{
    public class OrderCreatedDomainEventHandler : IAsyncNotificationHandler<OrderCreatedDomainEvent>
    {
        private readonly ILoggerFactory _logger;
        private readonly IBuyerRepository<Buyer> _buyerRepository;
        private readonly IIdentityService _identityService;

        public OrderCreatedDomainEventHandler(ILoggerFactory logger, IBuyerRepository<Buyer> buyerRepository, IIdentityService identityService)
        {
            _buyerRepository = buyerRepository ?? throw new ArgumentNullException(nameof(buyerRepository));
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(OrderCreatedDomainEvent orderNotification)
        {
            var cardTypeId = orderNotification.CardTypeId != 0 ? orderNotification.CardTypeId : 1;

            var buyerGuid = _identityService.GetUserIdentity();
            var buyer = await _buyerRepository.FindAsync(buyerGuid);

            if (buyer == null)
            {
                buyer = new Buyer(buyerGuid);
            }

            var payment = buyer.AddPaymentMethod(cardTypeId,
                $"Payment Method on {DateTime.UtcNow}",
                orderNotification.CardNumber,
                orderNotification.CardSecurityNumber,
                orderNotification.CardHolderName,
                orderNotification.CardExpiration,
                orderNotification.Order.Id);

            _buyerRepository.Add(buyer);

            await _buyerRepository.UnitOfWork
                .SaveEntitiesAsync();

            _logger.CreateLogger(nameof(OrderCreatedDomainEventHandler)).LogTrace($"A new payment method has been successfully added for orderId: {orderNotification.Order.Id}.");

        }
    }
}
