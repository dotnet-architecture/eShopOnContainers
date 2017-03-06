using System.Collections.Generic;

namespace eShopOnContainers.Core.Models.Catalog
{
    public class CatalogRoot
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int Count { get; set; }
        public List<CatalogItem> Data { get; set; }
    }
}
