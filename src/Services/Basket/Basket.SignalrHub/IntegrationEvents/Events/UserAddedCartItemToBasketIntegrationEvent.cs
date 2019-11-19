using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;
using System.Collections.Generic;

namespace Basket.SignalrHub.IntegrationEvents
{
    public class UserAddedCartItemToBasketIntegrationEvent : IntegrationEvent
    {
        public string BuyerName { get; set; }
        public BasketItem Item { get; set; }

        public UserAddedCartItemToBasketIntegrationEvent(string buyerName, BasketItem basketItem)
        {
            BuyerName = buyerName;
            Item = basketItem;
        }
    }
}
