using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.eShopOnContainers.Services.Common.Infrastructure.Catalog
{
    public class CatalogPriceChanged : IIntegrationEvent
    {        
        public int ItemId { get; private set; }

        public decimal NewPrice { get; private set; }

        public decimal OldPrice { get; set; }

        public CatalogPriceChanged(int itemId, decimal newPrice, decimal oldPrice)
        {
            ItemId = itemId;
            NewPrice = newPrice;
            OldPrice = oldPrice;
        }
}
}
