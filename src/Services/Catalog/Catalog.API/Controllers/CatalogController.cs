using Catalog.API.IntegrationEvents;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;
using Microsoft.eShopOnContainers.BuildingBlocks.IntegrationEventLogEF;
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
        private readonly CatalogContext _catalogContext;
        private readonly IOptionsSnapshot<Settings> _settings;
        private readonly IEventBus _eventBus;
        private readonly IIntegrationEventLogService _integrationEventLogService;

        public CatalogController(CatalogContext Context, IOptionsSnapshot<Settings> settings, IEventBus eventBus, IIntegrationEventLogService integrationEventLogService)
        {
            _catalogContext = Context;
            _settings = settings;
            _eventBus = eventBus;
            _integrationEventLogService = integrationEventLogService;

            ((DbContext)Context).ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        // GET api/v1/[controller]/items[?pageSize=3&pageIndex=10]
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> Items([FromQuery]int pageSize = 10, [FromQuery]int pageIndex = 0)

        {
            var totalItems = await _catalogContext.CatalogItems
                .LongCountAsync();

            var itemsOnPage = await _catalogContext.CatalogItems
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

            var totalItems = await _catalogContext.CatalogItems
                .Where(c => c.Name.StartsWith(name))
                .LongCountAsync();

            var itemsOnPage = await _catalogContext.CatalogItems
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
            var root = (IQueryable<CatalogItem>)_catalogContext.CatalogItems;

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
            var items = await _catalogContext.CatalogTypes
                .ToListAsync();

            return Ok(items);
        }

        // GET api/v1/[controller]/CatalogBrands
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> CatalogBrands()
        {
            var items = await _catalogContext.CatalogBrands
                .ToListAsync();

            return Ok(items);
        }

        //POST api/v1/[controller]/edit
        [Route("edit")]
        [HttpPost]
        public async Task<IActionResult> EditProduct([FromBody]CatalogItem productToUpdate)
        {
            var catalogItem = await _catalogContext.CatalogItems.SingleOrDefaultAsync(i => i.Id == productToUpdate.Id);
            if (catalogItem == null) return NotFound();

            bool raiseProductPriceChangedEvent = false;
            IntegrationEvent priceChangedEvent = null;

            if (catalogItem.Price != productToUpdate.Price) raiseProductPriceChangedEvent = true;
           
            if (raiseProductPriceChangedEvent) // Create event if price has changed
            {
                var oldPrice = catalogItem.Price;                
                priceChangedEvent = new ProductPriceChangedIntegrationEvent(catalogItem.Id, productToUpdate.Price, oldPrice);
            }

            //Update current product
            catalogItem = productToUpdate;

            // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
            using (var transaction = _catalogContext.Database.BeginTransaction())
            {
                    _catalogContext.CatalogItems.Update(catalogItem);
                    await _catalogContext.SaveChangesAsync();

                    //Save to EventLog only if product price changed
                    if(raiseProductPriceChangedEvent)
                        await _integrationEventLogService.SaveEventAsync(priceChangedEvent);

                    transaction.Commit();
            }

            //Publish to Event Bus only if product price changed
            if (raiseProductPriceChangedEvent)
            {
                _eventBus.Publish(priceChangedEvent);                                             
                await _integrationEventLogService.MarkEventAsPublishedAsync(priceChangedEvent);   
            }
                                                   
            return Ok();
        }

        //POST api/v1/[controller]/create
        [Route("create")]
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody]CatalogItem product)
        {
            _catalogContext.CatalogItems.Add(
                new CatalogItem
                {
                    CatalogBrandId = product.CatalogBrandId,
                    CatalogTypeId = product.CatalogTypeId,
                    Description = product.Description,
                    Name = product.Name,
                    PictureUri = product.PictureUri,
                    Price = product.Price
                });

            await _catalogContext.SaveChangesAsync();

            return Ok();
        }

        //DELETE api/v1/[controller]/id
        [Route("{id}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = _catalogContext.CatalogItems.SingleOrDefault(x => x.Id == id);

            if (product == null)
            {
                return NotFound();
            }            

            _catalogContext.CatalogItems.Remove(product);
            await _catalogContext.SaveChangesAsync();

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
