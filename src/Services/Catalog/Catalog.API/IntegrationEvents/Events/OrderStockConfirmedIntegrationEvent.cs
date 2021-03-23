﻿namespace Microsoft.eShopOnContainers.Services.Catalog.API.IntegrationEvents.Events
{
    using BuildingBlocks.EventBus.Events;

    public record OrderStockConfirmedIntegrationEvent : IntegrationEvent
    {
        public int OrderId { get; }

        public OrderStockConfirmedIntegrationEvent(int orderId) => OrderId = orderId;
    }
}