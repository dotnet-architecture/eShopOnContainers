using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.eShopWeb.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.eShopWeb.Services
{
    public interface ICatalogService
    {
        Task<Catalog> GetCatalogItems(int pageIndex, int itemsPage, int? brandID, int? typeId);
        Task<IEnumerable<SelectListItem>> GetBrands();
        Task<IEnumerable<SelectListItem>> GetTypes();
    }
}
