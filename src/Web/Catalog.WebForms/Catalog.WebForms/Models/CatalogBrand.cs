// Taken from https://github.com/dotnet/eShopOnContainers/blob/vs2017/src/Mobile/eShopOnContainers/eShopOnContainers.Core/Models/Catalog/CatalogBrand.cs
// Issue: How to make this DRY and still support the monolithic lift and shift scenario?

namespace eShopOnContainers.Core.Models.Catalog
{
    public class CatalogBrand
    {
        public int Id { get; set; }
        public string Brand { get; set; }

        public override string ToString() => Brand;
    }
}