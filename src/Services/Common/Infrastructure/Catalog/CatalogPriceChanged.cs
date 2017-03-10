using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.eShopOnContainers.Services.Common.Infrastructure.Catalog
{
    public class CatalogPriceChanged : IIntegrationEvent
    {
        public string Message { get { return "CatalogPriceChanged here!!"; } }

        public int ItemId { get; private set; }

        public decimal NewPrice { get; private set; }

        public CatalogPriceChanged(int itemId, decimal newPrice)
        {
            ItemId = itemId;
            NewPrice = newPrice;
        }
}
}
