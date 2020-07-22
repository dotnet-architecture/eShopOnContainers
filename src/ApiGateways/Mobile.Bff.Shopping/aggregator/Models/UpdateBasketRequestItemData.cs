namespace Microsoft.eShopOnContainers.Mobile.Shopping.HttpAggregator.Models
{

    public class UpdateBasketRequestItemData
    {
        public string Id { get; set; }          // Basket id

        public int ProductId { get; set; }      // Catalog item id

        public int Quantity { get; set; }       // Quantity
    }

}
