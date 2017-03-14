using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.eShopOnContainers.Services.Common.Infrastructure.Catalog
{
    public class ProductPriceChanged : IntegrationEventBase
    {        
        public int ItemId { get; private set; }

        public decimal NewPrice { get; private set; }

        public decimal OldPrice { get; set; }

        public ProductPriceChanged(int itemId, decimal newPrice, decimal oldPrice)
        {
            ItemId = itemId;
            NewPrice = newPrice;
            OldPrice = oldPrice;
        }
}
}
