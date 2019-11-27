using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;
using Microsoft.eShopOnContainers.Services.Basket.API.Model;
using System.Collections.Generic;

namespace Basket.API.IntegrationEvents.Events
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
