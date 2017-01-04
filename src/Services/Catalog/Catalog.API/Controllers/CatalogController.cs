
namespace Microsoft.eShopOnContainers.Services.Catalog.API.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.eShopOnContainers.Services.Catalog.API.Infrastructure;
    using Model;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using ViewModel;

    [Route("api/v1/[controller]")]
    public class CatalogController : ControllerBase
    {
        private readonly CatalogContext _context;

        public CatalogController(CatalogContext context)
        {
            _context = context;
        }

        // GET api/v1/[controller]/items/[?pageSize=3&pageIndex=10]
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> Items(int pageSize = 10, int pageIndex = 0)
        {
            var totalItems = await _context.CatalogItems
                 .LongCountAsync();

            var itemsOnPage = await _context.CatalogItems
                .OrderBy(c=>c.Name)
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToListAsync();

            var model = new PaginatedItemsViewModel<CatalogItem>(
                pageIndex, pageSize, totalItems, itemsOnPage);

            return Ok(model);
        }

        // GET api/v1/[controller]/items/withname/samplename
        [HttpGet]
        [Route("[action]/withname/{name:minlength(1)}")]
        public async Task<IActionResult> Items(string name, int pageSize = 10, int pageIndex = 0)
        {

            var totalItems = await _context.CatalogItems
                .Where(c => c.Name.StartsWith(name))
                .LongCountAsync();

            var itemsOnPage = await _context.CatalogItems
                .Where(c => c.Name.StartsWith(name))
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToListAsync();

            //itemsOnPage = ComposePicUri(itemsOnPage);

            var model = new PaginatedItemsViewModel<CatalogItem>(
                pageIndex, pageSize, totalItems, itemsOnPage);

            return Ok(model);
        }

        // GET api/v1/[controller]/items/type/1/brand/null
        [HttpGet]
        [Route("[action]/type/{catalogTypeId}/brand/{catalogBrandId}")]
        public async Task<IActionResult> Items(int? catalogTypeId, int? catalogBrandId, int pageSize = 10, int pageIndex = 0)
        {
            var root = (IQueryable<CatalogItem>)_context.CatalogItems;

            if (catalogTypeId.HasValue)
            {
                root = root.Where(ci => ci.CatalogTypeId == catalogTypeId);
            }

            if (catalogBrandId.HasValue)
            {
                root = root.Where(ci => ci.CatalogBrandId == catalogBrandId);
            }

            var totalItems = await root
                .LongCountAsync();

            var itemsOnPage = await root
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToListAsync();

            //itemsOnPage = ComposePicUri(itemsOnPage);

            var model = new PaginatedItemsViewModel<CatalogItem>(
                pageIndex, pageSize, totalItems, itemsOnPage);
            
            return Ok(model);
        }

        // GET api/v1/[controller]/CatalogTypes
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> CatalogTypes()
        {
            var items = await _context.CatalogTypes
                .ToListAsync();

            return Ok(items);
        }

        // GET api/v1/[controller]/CatalogBrands
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> CatalogBrands()
        {
            var items = await _context.CatalogBrands
                .ToListAsync();

            return Ok(items);
        }

        private List<CatalogItem> ComposePicUri(List<CatalogItem> items) {
            items.ForEach(x =>
            {
                x.PictureUri = x.PictureUri.Replace("localhost", Request.Host.Host);
            });

            return items;
        }
    }
}
