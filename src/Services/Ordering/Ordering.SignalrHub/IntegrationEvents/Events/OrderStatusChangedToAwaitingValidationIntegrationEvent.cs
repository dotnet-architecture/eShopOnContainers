﻿using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;

namespace Ordering.SignalrHub.IntegrationEvents
{
    public record OrderStatusChangedToAwaitingValidationIntegrationEvent : IntegrationEvent
    {
        public int OrderId { get; }
        public string OrderStatus { get; }
        public string BuyerName { get; }

        public OrderStatusChangedToAwaitingValidationIntegrationEvent(int orderId, string orderStatus, string buyerName)
        {
            OrderId = orderId;
            OrderStatus = orderStatus;
            BuyerName = buyerName;
        }
    }
}
