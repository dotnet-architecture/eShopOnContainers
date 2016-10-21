using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.eShopOnContainers.WebMVC.Models;

namespace Microsoft.eShopOnContainers.WebMVC.Services
{
    public class CatalogService : ICatalogService
    {
        List<CatalogItem> _items;

        public CatalogService() {
            _items = new List<CatalogItem>()
            {
                new CatalogItem() { Id = Guid.NewGuid(), Description = "Roslyn Red T-Shirt", Name = "Roslyn Red T-Shirt", Price = 12 },
                new CatalogItem() { Id = Guid.NewGuid(), Description = "Cupt Black & White Mug", Name = "Cupt Black & White Mug", Price= 17 },
                new CatalogItem() { Id = Guid.NewGuid(), Description = "Prism White T-Shirt", Name = "Prism White T-Shirt", Price = 12 },
                new CatalogItem() { Id = Guid.NewGuid(), Description = ".NET Bot Black Sweatshirt", Name = ".NET Bot Black Sweatshirt", Price = decimal.Parse("19.5") }
            };
        }
         
        public CatalogItem GetCatalogItem(Guid Id)
        {
            return _items.Where(x => x.Id == Id).FirstOrDefault();
        }

        public Task<List<CatalogItem>> GetCatalogItems()
        {
            return Task.Run(() => { return _items; });
        }
    }
}
