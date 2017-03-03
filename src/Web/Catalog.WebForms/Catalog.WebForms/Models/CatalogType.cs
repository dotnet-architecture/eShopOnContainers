// Taken from https://github.com/dotnet/eShopOnContainers/blob/vs2017/src/Mobile/eShopOnContainers/eShopOnContainers.Core/Models/Catalog/CatalogType.cs
// Issue: How to make this DRY and still support the monolithic lift and shift scenario?
namespace eShopOnContainers.Core.Models.Catalog
{
    public class CatalogType
    {
        public int Id { get; set; }
        public string Type { get; set; }

        public override string ToString() => Type;
    }
}
