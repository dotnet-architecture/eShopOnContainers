namespace Microsoft.eShopOnContainers.Services.Ordering.API.Models
{
    //TO DO: Confirm if this class is not needed, if not, remove it
    //(CDLTLL)
    public class OrderItemViewModel
    {
        public int ProductId { get;  set; }

        public string ProductName { get;  set; }

        public decimal UnitPrice { get;  set; }

        public decimal Discount { get;  set; }

        public int Units { get;  set; }

        public string PictureUrl { get; set; }
    }
}