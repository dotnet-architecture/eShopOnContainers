namespace Microsoft.eShopOnContainers.WebMVC.ViewModels
{
    public record CatalogItem
    {
        public int Id { get; init; }
        public string Name { get; init; }
        public string Description { get; init; }
        public decimal Price { get; init; }
        public string PictureUri { get; init; }
        public int CatalogBrandId { get; init; }
        public string CatalogBrand { get; init; }
        public int CatalogTypeId { get; init; }
        public string CatalogType { get; init; }
    }
}