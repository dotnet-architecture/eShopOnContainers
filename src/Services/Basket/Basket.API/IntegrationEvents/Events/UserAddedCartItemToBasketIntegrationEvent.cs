using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;
using Microsoft.eShopOnContainers.Services.Basket.API.Model;
using System.Collections.Generic;

namespace Basket.API.IntegrationEvents.Events
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
