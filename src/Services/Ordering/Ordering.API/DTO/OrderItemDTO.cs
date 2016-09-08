using System;
using System.Runtime.Serialization;


namespace Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel.Order
{
    [DataContract]
    public sealed class OrderItemDTO
    {
        public OrderItemDTO() { }

        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public int Quantity { get; set; }

        [DataMember]
        public int FulfillmentRemaining { get; set; }

        public override string ToString()
        {
            return String.Format("ID: {0}, Quantity: {1}, Fulfillment Remaing: {2}", this.ItemId, this.Quantity, this.FulfillmentRemaining);
        }
    }
}
