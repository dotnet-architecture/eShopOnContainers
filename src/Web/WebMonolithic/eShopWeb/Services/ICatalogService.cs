using eShopWeb.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eShopWeb.Services
{
    public interface ICatalogService
    {
        IList<CatalogItem> GetCatalogItems(int page, int itemsPage, int? brandFilterApplied, int? typesFilterApplied);
        IEnumerable<SelectListItem> GetBrands();
        IEnumerable<SelectListItem> GetTypes();
    }
}
