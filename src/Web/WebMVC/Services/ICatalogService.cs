using Microsoft.eShopOnContainers.WebMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.eShopOnContainers.WebMVC.Services
{
    public interface ICatalogService
    {
        Task<List<CatalogItem>> GetCatalogItems();
        CatalogItem GetCatalogItem(Guid Id);
    }
}
