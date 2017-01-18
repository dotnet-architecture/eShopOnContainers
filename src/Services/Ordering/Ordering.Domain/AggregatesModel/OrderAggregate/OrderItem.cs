namespace Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel.OrderAggregate
{
    using Microsoft.eShopOnContainers.Services.Ordering.Domain.SeedWork;


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
