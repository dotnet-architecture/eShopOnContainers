namespace eShopOnContainers.Core.Models.Catalog
{
    public class CatalogBrand
    {
        public int Id { get; set; }
        public string Brand { get; set; }

        public override string ToString()
        {
            return Brand;
        }
    }
}