using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;
using System.Collections.Generic;

namespace Ordering.SignalrHub.IntegrationEvents
{
    public class UserAddedCatalogItemToBasketIntegrationEvent : IntegrationEvent
    {
        public string BuyerName { get; set; }
        public int BasketItemCount { get; set; }

        public UserAddedCatalogItemToBasketIntegrationEvent(string buyerName, int basketItemCount)
        {
            BuyerName = buyerName;
            BasketItemCount = basketItemCount;
        }
    }
}
