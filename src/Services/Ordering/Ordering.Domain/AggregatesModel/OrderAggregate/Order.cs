namespace Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel.OrderAggregate
{
    using Microsoft.eShopOnContainers.Services.Ordering.Domain.SeedWork;
    using Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel.BuyerAggregate;
    using System;
    using System.Collections.Generic;

    //(CDLTLL)
    //TO DO: Need to add additional Domain Logic to this Aggregate-Root for 
    //scenarios related to Order state changes, stock availability validation, etc.
    public class Order
        : Entity, IAggregateRoot
    {
        public int BuyerId { get; private set; }

        public Buyer Buyer { get; private set; }

        public DateTime OrderDate { get; private set; }

        public int StatusId { get; private set; }

        public OrderStatus Status { get; private set; }

        public ICollection<OrderItem> OrderItems { get; private set; }

        public int? ShippingAddressId { get; private set; }

        public Address ShippingAddress { get; private set; }

        public int PaymentId { get; private set; }

        public Payment Payment { get; private set; }

        protected Order() { }

        public Order(int buyerId, int paymentId)
        {
            BuyerId = buyerId;
            PaymentId = paymentId;
            StatusId = OrderStatus.InProcess.Id;
            OrderDate = DateTime.UtcNow;
            OrderItems = new List<OrderItem>();
        }

        public void SetAddress(Address address)
        {
            if (address == null)
            {
                throw new ArgumentNullException(nameof(address));
            }

            ShippingAddress = address;
        }

        //TO DO: 
        // (CDLTLL) Bad implementation, needs to be changed.
        // The AddOrderItem should have the needed data params 
        // instead of an already created OrderItem object.
        // The Aggregate-Root is responsible for any Update/Creation of its child entities
        // If we are providing an already created OrderItem, that was created from the outside
        // and the AggregateRoot cannot control/validate any rule/invariants/consistency. 
        public void AddOrderItem(OrderItem item)
        {
            //TO DO: Bad implementation, need to change.
            // The code "new OrderItem(params);" should be done here
            // Plus any validation/rule related
            OrderItems.Add(item);

            //(CDLTLL)
            // TO DO: Some more logic needs to be added here,
            // Like consolidating items that are the same product in one single OrderItem with several units
            // Also validation logic could be added here (like ensuring it is adding at least one item unit)

            //Or, If there are different amounts of discounts per added OrderItem 
            //but the product Id is the same to existing Order Items, you should 
            //apply the higher discount, or any other domain logic that makes sense.
        }
    }
}
