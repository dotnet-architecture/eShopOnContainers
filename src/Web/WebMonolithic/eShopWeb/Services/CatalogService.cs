using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eShopWeb.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace eShopWeb.Services
{
    public class CatalogService : ICatalogService
    {
        public IEnumerable<SelectListItem> GetBrands()
        {
            throw new NotImplementedException();
        }

        public IList<CatalogItem> GetCatalogItems(int page, int itemsPage, int? brandFilterApplied, int? typesFilterApplied)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<SelectListItem> GetTypes()
        {
            throw new NotImplementedException();
        }
    }
}
