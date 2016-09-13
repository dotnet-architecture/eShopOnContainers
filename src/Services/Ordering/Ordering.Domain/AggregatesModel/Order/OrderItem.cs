using System;
using System.Runtime.Serialization;

using Microsoft.eShopOnContainers.Services.Ordering.Domain.SeedWork;

namespace Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel
{
    public class OrderItem : Entity
    {
        public OrderItem() { } // Infrastructure. EF might need a plain constructor. Do not use.

        //NOTE: The OrderItem Id (Id) comes from the Entity base class

        public Guid ProductId { get; set; }

        public Guid OrderId { get; set; }

        public decimal UnitPrice { get; set; }

        public int Quantity { get; set; }

        public decimal Discount { get; set; }

        public int FulfillmentRemaining { get; set; }

        public override string ToString()
        {
            return String.Format("Product Id: {0}, Quantity: {1}, Fulfillment Remaing: {2}", this.Id, this.Quantity, this.FulfillmentRemaining);
        }
    }
}
