using System.Collections.Generic;

namespace Microsoft.eShopOnContainers.WebMVC.ViewModels
{
    public record Catalog
    {
        public int PageIndex { get; init; }
        public int PageSize { get; init; }
        public int Count { get; init; }
        public List<CatalogItem> Data { get; init; }
    }
}
