using Coupon.API.Infrastructure;
using Coupon.API.IntegrationEvents.Events;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;

namespace Coupon.API.IntegrationEvents.EventHandling
{
    public class OrderStatusChangedToAwaitingCouponValidationIntegrationEventHandler
        : IIntegrationEventHandler<OrderStatusChangedToAwaitingCouponValidationIntegrationEvent>
    {
        private readonly ILogger<OrderStatusChangedToAwaitingCouponValidationIntegrationEventHandler> _logger;
        private readonly ICouponRepository _repository;
        private readonly IEventBus _eventBus;

        public OrderStatusChangedToAwaitingCouponValidationIntegrationEventHandler(
            ILogger<OrderStatusChangedToAwaitingCouponValidationIntegrationEventHandler> logger,
            ICouponRepository repository,
            IEventBus eventBus)
        {
            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
            _repository = repository ?? throw new System.ArgumentNullException(nameof(repository));
            _eventBus = eventBus ?? throw new System.ArgumentNullException(nameof(eventBus));
        }

        public async Task Handle(OrderStatusChangedToAwaitingCouponValidationIntegrationEvent @event)
        {
            var coupon = await _repository.FindByCodeAsync(@event.CouponCode);
            //add validation of coupon as string
            if (coupon == null)
            {
                var newCoupon = new Infrastructure.Models.Coupon
                {
                    Code = @event.CouponCode,
                    Consumed = true,
                    Discount = int.Parse(@event.CouponCode.Split("-").Last()),
                    OrderId = @event.OrderId
                };

                await _repository.AddAsync(newCoupon);
                var orderCouponConfirmedIntegrationEvent = new OrderCouponConfirmedIntegrationEvent(@event.OrderId);
                _eventBus.Publish(orderCouponConfirmedIntegrationEvent);
            }
            else if (coupon.Consumed)
            {
                var orderCouponRejectedIntegrationEvent = new OrderCouponRejectedIntegrationEvent(@event.OrderId);
                _eventBus.Publish(orderCouponRejectedIntegrationEvent);
            }
            else
            {
                coupon.Consumed = true;
                coupon.OrderId = @event.OrderId;
                await _repository.UpdateAsync(coupon);

                var orderCouponConfirmedIntegrationEvent = new OrderCouponConfirmedIntegrationEvent(@event.OrderId);
                _eventBus.Publish(orderCouponConfirmedIntegrationEvent);
            }
        }
    }
}
