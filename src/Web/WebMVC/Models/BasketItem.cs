using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.eShopOnContainers.WebMVC.Models
{
    public class BasketItem
    {
        public string Id { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public string PictureUrl { get; set; }
    }
}
