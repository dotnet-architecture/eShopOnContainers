using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.eShopOnContainers.Services.Basket.API.Model
{
    public class BasketItem
    {
        public Guid Id { get; set; }
        public decimal Price { get; set; }
        public int Count { get; set; }
    }
}
