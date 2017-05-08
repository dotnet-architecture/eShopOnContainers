using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;
using Microsoft.eShopOnContainers.Services.Basket.API.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Basket.API.IntegrationEvents.Events
{
    public class UserCheckoutAccepted : IntegrationEvent
    {
        public string UserId {get; }
        CustomerBasket Basket { get; }
        public UserCheckoutAccepted(string userId, CustomerBasket basket)
        {
            UserId = userId;
            Basket = basket;
        }

    }
}
