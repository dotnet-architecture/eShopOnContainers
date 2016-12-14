namespace Microsoft.eShopOnContainers.Services.Ordering.API.Models
{
    public class OrderItemViewModel
    {
        public int ProductId { get;  set; }

        public string ProductName { get;  set; }

        public decimal UnitPrice { get;  set; }

        public decimal Discount { get;  set; }

        public int Units { get;  set; }
    }
}