using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.eShopWeb.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.eShopWeb.Infrastructure;
using Microsoft.eShopWeb.ViewModels;

namespace Microsoft.eShopWeb.Services
{
    public class CatalogService : ICatalogService
    {
        private readonly CatalogContext _context;
        private readonly IOptionsSnapshot<CatalogSettings> _settings;
        
        public CatalogService(CatalogContext context, IOptionsSnapshot<CatalogSettings> settings)
        {
            _context = context;
            _settings = settings;            
        }

        public async Task<Catalog> GetCatalogItems(int pageIndex, int itemsPage, int? brandId, int? typeId)
        {
            var root = (IQueryable<CatalogItem>)_context.CatalogItems;

            if (typeId.HasValue)
            {
                root = root.Where(ci => ci.CatalogTypeId == typeId);
            }

            if (brandId.HasValue)
            {
                root = root.Where(ci => ci.CatalogBrandId == brandId);
            }

            var totalItems = await root
                .LongCountAsync();

            var itemsOnPage = await root
                .Skip(itemsPage * pageIndex)
                .Take(itemsPage)
                .ToListAsync();

            itemsOnPage = ComposePicUri(itemsOnPage);

            return new Catalog() { Data = itemsOnPage, PageIndex = pageIndex, Count = (int)totalItems };           
        }

        public async Task<IEnumerable<SelectListItem>> GetBrands()
        {
            var brands = await _context.CatalogBrands.ToListAsync();
            var items = new List<SelectListItem>();
            items.Add(new SelectListItem() { Value = null, Text = "All", Selected = true });
            foreach (CatalogBrand brand in brands)
            {
                items.Add(new SelectListItem() { Value = brand.Id.ToString(), Text = brand.Brand });
            }

            return items;
        }

        public async Task<IEnumerable<SelectListItem>> GetTypes()
        {
            var types = await _context.CatalogTypes.ToListAsync();
            var items = new List<SelectListItem>();
            items.Add(new SelectListItem() { Value = null, Text = "All", Selected = true });
            foreach (CatalogType type in types)
            {
                items.Add(new SelectListItem() { Value = type.Id.ToString(), Text = type.Type });
            }

            return items;
        }

        private List<CatalogItem> ComposePicUri(List<CatalogItem> items)
        {
            var baseUri = _settings.Value.CatalogBaseUrl;                      
            items.ForEach(x =>
            {
                x.PictureUri = x.PictureUri.Replace("http://catalogbaseurltobereplaced", baseUri);
            });

            return items;
        }
    }
}
