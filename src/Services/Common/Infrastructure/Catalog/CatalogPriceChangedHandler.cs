using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.eShopOnContainers.Services.Common.Infrastructure.Catalog
{
    public class CatalogPriceChangedHandler : IIntegrationEventHandler<CatalogPriceChanged>
    {
        public void Handle(CatalogPriceChanged @event)
        {
            throw new NotImplementedException();
        }
    }
}
