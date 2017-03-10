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
        public CatalogPriceChangedHandler()
        {
            //_repository = repository;
        }

        public void Handle(CatalogPriceChanged @event)
        {
            
        }        
    }
}

