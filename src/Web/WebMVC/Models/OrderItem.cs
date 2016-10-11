using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.eShopOnContainers.WebMVC.Models
{
    public class OrderItem
    {
        Guid Id;
        public Guid ProductId { get; set; }
        public Guid OrderId { get; set; }
        public string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal Discount { get; set; }
        public override string ToString()
        {
            return String.Format("Product Id: {0}, Quantity: {1}", this.Id, this.Quantity);
        }
    }

}
