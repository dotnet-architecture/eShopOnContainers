namespace Microsoft.eShopOnContainers.Mobile.Shopping.HttpAggregator.Models
{
    public class CatalogItem
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public string PictureUri { get; set; }
    }
}
