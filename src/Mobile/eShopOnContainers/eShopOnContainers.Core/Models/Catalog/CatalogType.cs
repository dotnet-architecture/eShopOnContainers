namespace eShopOnContainers.Core.Models.Catalog
{
    public class CatalogType
    {
        public int CatalogTypeId { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}