using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events.IntegrationEventLog;
using Microsoft.eShopOnContainers.Services.Catalog.API.Infrastructure;
using Microsoft.eShopOnContainers.Services.Catalog.API.IntegrationEvents.Events;
using Microsoft.eShopOnContainers.Services.Catalog.API.Model;
using Microsoft.eShopOnContainers.Services.Catalog.API.ViewModel;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.eShopOnContainers.Services.Catalog.API.Controllers
{
    [Route("api/v1/[controller]")]
    public class CatalogController : ControllerBase
    {
        private readonly CatalogContext _context;
        private readonly IOptionsSnapshot<Settings> _settings;
        private readonly IEventBus _eventBus;

        public CatalogController(CatalogContext context, IOptionsSnapshot<Settings> settings, IEventBus eventBus)
        {
            _context = context;
            _settings = settings;
            _eventBus = eventBus;

            ((DbContext)context).ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        // GET api/v1/[controller]/items[?pageSize=3&pageIndex=10]
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> Items([FromQuery]int pageSize = 10, [FromQuery]int pageIndex = 0)
        {
            var totalItems = await _context.CatalogItems
                .LongCountAsync();

            var itemsOnPage = await _context.CatalogItems
                .OrderBy(c=>c.Name)
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToListAsync();

            itemsOnPage = ComposePicUri(itemsOnPage);

            var model = new PaginatedItemsViewModel<CatalogItem>(
                pageIndex, pageSize, totalItems, itemsOnPage);           

            return Ok(model);
        }

        // GET api/v1/[controller]/items/withname/samplename[?pageSize=3&pageIndex=10]
        [HttpGet]
        [Route("[action]/withname/{name:minlength(1)}")]
        public async Task<IActionResult> Items(string name, [FromQuery]int pageSize = 10, [FromQuery]int pageIndex = 0)
        {

            var totalItems = await _context.CatalogItems
                .Where(c => c.Name.StartsWith(name))
                .LongCountAsync();

            var itemsOnPage = await _context.CatalogItems
                .Where(c => c.Name.StartsWith(name))
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToListAsync();

            itemsOnPage = ComposePicUri(itemsOnPage);

            var model = new PaginatedItemsViewModel<CatalogItem>(
                pageIndex, pageSize, totalItems, itemsOnPage);

            return Ok(model);
        }

        // GET api/v1/[controller]/items/type/1/brand/null[?pageSize=3&pageIndex=10]
        [HttpGet]
        [Route("[action]/type/{catalogTypeId}/brand/{catalogBrandId}")]
        public async Task<IActionResult> Items(int? catalogTypeId, int? catalogBrandId, [FromQuery]int pageSize = 10, [FromQuery]int pageIndex = 0)
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

            itemsOnPage = ComposePicUri(itemsOnPage);

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

        [Route("edit")]
        [HttpPost]
        public async Task<IActionResult> EditProduct([FromBody]CatalogItem product)
        {
            var item = await _context.CatalogItems.SingleOrDefaultAsync(i => i.Id == product.Id);

            if (item == null)
            {
                return NotFound();
            }

            if (item.Price != product.Price)
            {
                var oldPrice = item.Price;
                item.Price = product.Price;
                _context.CatalogItems.Update(item);

                var @event = new ProductPriceChangedIntegrationEvent(item.Id, item.Price, oldPrice);
                var eventLogEntry = new IntegrationEventLogEntry(@event);                
                _context.IntegrationEventLog.Add(eventLogEntry);                

                await _context.SaveChangesAsync();
                
                _eventBus.Publish(@event);
                
                eventLogEntry.TimesSent++;
                eventLogEntry.State = EventStateEnum.Published;
                _context.IntegrationEventLog.Update(eventLogEntry);
                await _context.SaveChangesAsync();
            }         

            return Ok();
        }

        [Route("create")]
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody]CatalogItem product)
        {
            _context.CatalogItems.Add(
                new CatalogItem
                {
                    CatalogBrandId = product.CatalogBrandId,
                    CatalogTypeId = product.CatalogTypeId,
                    Description = product.Description,
                    Name = product.Name,
                    PictureUri = product.PictureUri,
                    Price = product.Price
                });

            await _context.SaveChangesAsync();

            return Ok();
        }

        [Route("{id}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = _context.CatalogItems.SingleOrDefault(x => x.Id == id);

            if (product == null)
            {
                return NotFound();
            }            

            _context.CatalogItems.Remove(product);
            await _context.SaveChangesAsync();

            return Ok();
        }

        private List<CatalogItem> ComposePicUri(List<CatalogItem> items) {
            var baseUri = _settings.Value.ExternalCatalogBaseUrl;
            items.ForEach(x =>
            {
                x.PictureUri = x.PictureUri.Replace("http://externalcatalogbaseurltobereplaced", baseUri);
            });

            return items;
        }
    }
}
