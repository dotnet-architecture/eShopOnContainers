using System.Threading.Tasks;
using Coupon.API.Infrastructure.Repositories;
using Coupon.API.IntegrationEvents.Events;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;
using Serilog;
using Serilog.Context;

namespace Coupon.API.IntegrationEvents.EventHandlers
{
    public class OrderStatusChangedToAwaitingCouponValidationIntegrationEventHandler : IIntegrationEventHandler<OrderStatusChangedToAwaitingCouponValidationIntegrationEvent>
    {
        private readonly ICouponRepository _couponRepository;
        private readonly IEventBus _eventBus;

        public OrderStatusChangedToAwaitingCouponValidationIntegrationEventHandler(ICouponRepository couponRepository, IEventBus eventBus)
        {
            _couponRepository = couponRepository;
            _eventBus = eventBus;
        }

        public async Task Handle(OrderStatusChangedToAwaitingCouponValidationIntegrationEvent @event)
        {
            await Task.Delay(3000);

            using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}-Coupon.API"))
            {
                Log.Information("----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", @event.Id, "Coupon.API", @event);

                var couponIntegrationEvent = await ProcessIntegrationEventAsync(@event);

                Log.Information("----- Publishing integration event: {IntegrationEventId} from {AppName} - ({@IntegrationEvent})", couponIntegrationEvent.Id, "Coupon.API", couponIntegrationEvent);

                _eventBus.Publish(couponIntegrationEvent);
            }
        }

        private async Task<IntegrationEvent> ProcessIntegrationEventAsync(OrderStatusChangedToAwaitingCouponValidationIntegrationEvent integrationEvent)
        {
            var coupon = await _couponRepository.FindCouponByCodeAsync(integrationEvent.Code);

            Log.Information("----- Coupon \"{CouponCode}\": {@Coupon}", integrationEvent.Code, coupon);

            if (coupon == null || coupon.Consumed)
            {
                return new OrderCouponRejectedIntegrationEvent(integrationEvent.OrderId, coupon.Code);
            }

            Log.Information("Consumed coupon: {DiscountCode}", integrationEvent.Code);

            await _couponRepository.UpdateCouponConsumedByCodeAsync(integrationEvent.Code, integrationEvent.OrderId);

            return new OrderCouponConfirmedIntegrationEvent(integrationEvent.OrderId, coupon.Discount);
        }
    }
}
