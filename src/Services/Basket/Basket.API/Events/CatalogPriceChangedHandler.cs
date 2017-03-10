using Microsoft.eShopOnContainers.Services.Basket.API.Model;
using Microsoft.eShopOnContainers.Services.Common.Infrastructure;
using Microsoft.eShopOnContainers.Services.Common.Infrastructure.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Basket.API.Events
{
    public class CatalogPriceChangedHandler : IIntegrationEventHandler<CatalogPriceChanged>
    {
        private readonly IBasketRepository _repository;
        public CatalogPriceChangedHandler(IBasketRepository repository)
        {
            _repository = repository;
        }

        public async Task Handle(CatalogPriceChanged @event)
        {
            var userIds = await _repository.GetUsers();
            foreach (var id in userIds)
            {
                var basket = await _repository.GetBasket(id);
                await UpdateBasket(@event.ItemId, @event.NewPrice, basket);
            }
        }

        private async Task UpdateBasket(int itemId, decimal newPrice, CustomerBasket basket)
        {
            //TODO here seems to be a problem with the format
            var itemsToUpdate = basket?.Items?.Where(x => int.Parse(x.Id) == itemId).ToList();
            if (itemsToUpdate != null)
            {
                foreach (var item in itemsToUpdate)
                {
                    var originalPrice = item.UnitPrice;
                    item.UnitPrice = newPrice;
                    item.OldUnitPrice = originalPrice;
                }
                await _repository.UpdateBasket(basket);
            }
        }
    }
}

