// Taken from https://github.com/dotnet/eShopOnContainers/blob/vs2017/src/Mobile/eShopOnContainers/eShopOnContainers.Core/Models/Catalog/CatalogItem.cs
// Issue: How to make this DRY and still support the monolithic lift and shift scenario?
namespace eShopOnContainers.Core.Models.Catalog
{
    public class CatalogItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string PictureUri { get; set; }
        public int CatalogBrandId { get; set; }
        public string CatalogBrand { get; set; }
        public int CatalogTypeId { get; set; }
        public string CatalogType { get; set; }
    }
}