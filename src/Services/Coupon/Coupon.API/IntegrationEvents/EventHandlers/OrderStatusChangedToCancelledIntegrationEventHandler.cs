using System.Threading.Tasks;
using Coupon.API.Infrastructure.Repositories;
using Coupon.API.IntegrationEvents.Events;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;

namespace Coupon.API.IntegrationEvents.EventHandlers
{
    public class OrderStatusChangedToCancelledIntegrationEventHandler : IIntegrationEventHandler<OrderStatusChangedToCancelledIntegrationEvent>
    {
        private readonly ICouponRepository _couponRepository;

        public OrderStatusChangedToCancelledIntegrationEventHandler(ICouponRepository couponRepository)
        {
            _couponRepository = couponRepository;
        }

        public async Task Handle(OrderStatusChangedToCancelledIntegrationEvent @event)
        {
            await _couponRepository.UpdateCouponReleasedByOrderIdAsync(@event.OrderId);
        }
    }
}
