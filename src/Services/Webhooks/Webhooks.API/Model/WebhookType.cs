using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Webhooks.API.Model
{
    public enum WebhookType
    {
        CatalogItemPriceChange = 1,
        OrderShipped = 2,
        OrderPaid = 3
    }
}
