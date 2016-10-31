using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.eShopOnContainers.WebMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.eShopOnContainers.WebMVC.Services
{
    public interface ICatalogService
    {
        int TotalItems { get; }

        Task<List<CatalogItem>> GetCatalogItems(int? skip, int? take);
        CatalogItem GetCatalogItem(string Id);
        IEnumerable<SelectListItem> GetBrands();
        IEnumerable<SelectListItem> GetTypes();
    }
}
