namespace Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel.OrderAggregate
{
    using Microsoft.eShopOnContainers.Services.Ordering.Domain.SeedWork;

    //TO DO:
    //(CDLTLL) Wrong implementation. Need to put Setters as private
    // and only be able to update the OrderItem through specific methods, if needed, so we can
    // have validations/control/logic in those "update or set methods".
    //We also need to have a constructor with the needed params, we must not use the "setters"..  
    public class OrderItem
        :Entity
    {
        public int ProductId { get;  set; }

        public string ProductName { get;  set; }

        public string PictureUrl { get; set; }

        public int OrderId { get;  set; }

        public decimal UnitPrice { get;  set; }

        public decimal Discount { get;  set; }

        public int Units { get;  set; }

    }
}
