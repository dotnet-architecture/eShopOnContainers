namespace eShopOnContainers.Core.Models.Catalog
{
    public class CatalogBrand
    {
        public int CatalogBrandId { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}