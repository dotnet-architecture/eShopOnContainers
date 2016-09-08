using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel
{
    public enum OrderStatus
    {
        Unknown,
        New,
        Submitted,
        InProcess,
        Backordered,
        Shipped,
        Canceled,
    }
}
