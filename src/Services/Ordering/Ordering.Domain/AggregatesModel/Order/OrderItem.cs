using System;
using System.Runtime.Serialization;

using Microsoft.eShopOnContainers.Services.Ordering.Domain.SeedWork;

namespace Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel
{
    public class OrderItem : Entity
    {
        //(CDLTLL) Might remove this constructor so
        // just with a method in the Order-AggregateRoot you can add an OrderItem..
        //public OrderItem(Guid itemId, decimal itemPrice, int quantity)
        //{
        //    this.Id = itemId;
        //    this.ItemPrice = itemPrice;
        //    this.Quantity = quantity;
        //    this.FulfillmentRemaining = quantity;  <---- Put this logic into the AggregateRoot method AddOrderItem()
        //}

        protected OrderItem() { } // Infrastructure. EF might need a plain constructor. Do not use.

        public Guid ItemId { get; set; }

        public decimal ItemPrice { get; set; }

        public int Quantity { get; set; }

        public int FulfillmentRemaining { get; set; }

        public override string ToString()
        {
            return String.Format("ID: {0}, Quantity: {1}, Fulfillment Remaing: {2}", this.ItemId, this.Quantity, this.FulfillmentRemaining);
        }
    }
}
