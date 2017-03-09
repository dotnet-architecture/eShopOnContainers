using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.eShopOnContainers.Services.Common.Infrastructure.Catalog
{
    public class CatalogPriceChanged : IIntegrationEvent
    {
        private readonly string _eventName = "catalogpricechanged";

        public string Name {
            get
            {
                return _eventName;
            }
        }

        public string Message { get { return "CatalogPriceChanged!!"; } }
    }
}
