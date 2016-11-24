namespace Microsoft.eShopOnContainers.Services.Ordering.Domain
{
    using Microsoft.eShopOnContainers.Services.Ordering.Domain.SeedWork;


    public class OrderItem
        :Entity
    {
        public int ProductId { get; private set; }

        public string ProductName { get; private set; }

        public int OrderId { get; private set; }

        public decimal UnitPrice { get; private set; }

        public decimal Discount { get; private set; }

        public int Units { get; private set; }

        protected OrderItem()
        {
        }
    }
}
