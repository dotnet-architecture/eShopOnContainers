using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.eShopOnContainers.Services.Catalog.API.Model
{
    public class CatalogItem
    {
        public CatalogItem()
        {
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int ImageCount { get; set; }
    }
}
