using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CatalogApi;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.eShopOnContainers.Services.Catalog.API;
using Microsoft.eShopOnContainers.Services.Catalog.API.Infrastructure;
using Microsoft.eShopOnContainers.Services.Catalog.API.Model;
using Microsoft.eShopOnContainers.Services.Catalog.API.ViewModel;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using static CatalogApi.Catalog;

namespace Catalog.API.Grpc
{
    public class CatalogService : CatalogBase
    {
        private readonly CatalogContext _catalogContext;
        private readonly CatalogSettings _settings;
        private readonly ILogger _logger;
        public CatalogService(CatalogContext dbContext, IOptions<CatalogSettings> settings, ILogger<CatalogService> logger)
        {
            _settings = settings.Value;
            _catalogContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _logger = logger;
        }

        public override async Task<CatalogItemResponse> GetItemById(CatalogItemRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Begin grpc call CatalogService.GetItemById for product id {Id}", request.Id);
            if (request.Id <= 0)
            {
                context.Status = new Status(StatusCode.FailedPrecondition, $"Id must be > 0 (received {request.Id})");
                return null;
            }

            var item = await _catalogContext.CatalogItems.SingleOrDefaultAsync(ci => ci.Id == request.Id);
            var baseUri = _settings.PicBaseUrl;
            var azureStorageEnabled = _settings.AzureStorageEnabled;
            item.FillProductUrl(baseUri, azureStorageEnabled: azureStorageEnabled);

            if (item != null)
            {
                return new CatalogItemResponse()
                {
                    AvailableStock = item.AvailableStock,
                    Description = item.Description,
                    Id = item.Id,
                    MaxStockThreshold = item.MaxStockThreshold,
                    Name = item.Name,
                    OnReorder = item.OnReorder,
                    PictureFileName = item.PictureFileName,
                    PictureUri = item.PictureUri,
                    Price = (double)item.Price,
                    RestockThreshold = item.RestockThreshold
                };
            }

            context.Status = new Status(StatusCode.NotFound, $"Product with id {request.Id} do not exist");
            return null;
        }

        public override async Task<PaginatedItemsResponse> GetItemsByIds(CatalogItemsRequest request, ServerCallContext context)
        {
            if (!string.IsNullOrEmpty(request.Ids))
            {
                var items = await GetItemsByIdsAsync(request.Ids);

                context.Status = !items.Any() ?
                    new Status(StatusCode.NotFound, $"ids value invalid. Must be comma-separated list of numbers") :
                    new Status(StatusCode.OK, string.Empty);

                return this.MapToResponse(items);
            }

            var totalItems = await _catalogContext.CatalogItems
                .LongCountAsync();

            var itemsOnPage = await _catalogContext.CatalogItems
                .OrderBy(c => c.Name)
                .Skip(request.PageSize * request.PageIndex)
                .Take(request.PageSize)
                .ToListAsync();

            /* The "awesome" fix for testing Devspaces */

            /*
            foreach (var pr in itemsOnPage) {
                pr.Name = "Awesome " + pr.Name;
            }

            */

            itemsOnPage = ChangeUriPlaceholder(itemsOnPage);

            var model = this.MapToResponse(itemsOnPage, totalItems, request.PageIndex, request.PageSize);
            context.Status = new Status(StatusCode.OK, string.Empty);

            return model;
        }

        private PaginatedItemsResponse MapToResponse(List<CatalogItem> items)
        {
            return this.MapToResponse(items, items.Count, 1, items.Count);
        }

        private PaginatedItemsResponse MapToResponse(List<CatalogItem> items, long count, int pageIndex, int pageSize)
        {
            var result = new PaginatedItemsResponse()
            {
                Count = count,
                PageIndex = pageIndex,
                PageSize = pageSize,
            };

            items.ForEach(i =>
            {
                var brand = i.CatalogBrand == null
                            ? null
                            : new CatalogApi.CatalogBrand()
                            {
                                Id = i.CatalogBrand.Id,
                                Name = i.CatalogBrand.Brand,
                            };
                var catalogType = i.CatalogType == null
                                  ? null
                                  : new CatalogApi.CatalogType()
                                  {
                                      Id = i.CatalogType.Id,
                                      Type = i.CatalogType.Type,
                                  };

                result.Data.Add(new CatalogItemResponse()
                {
                    AvailableStock = i.AvailableStock,
                    Description = i.Description,
                    Id = i.Id,
                    MaxStockThreshold = i.MaxStockThreshold,
                    Name = i.Name,
                    OnReorder = i.OnReorder,
                    PictureFileName = i.PictureFileName,
                    PictureUri = i.PictureUri,
                    RestockThreshold = i.RestockThreshold,
                    CatalogBrand = brand,
                    CatalogType = catalogType,
                    Price = (double)i.Price,
                });
            });

            return result;
        }


        private async Task<List<CatalogItem>> GetItemsByIdsAsync(string ids)
        {
            var numIds = ids.Split(',').Select(id => (Ok: int.TryParse(id, out int x), Value: x));

            if (!numIds.All(nid => nid.Ok))
            {
                return new List<CatalogItem>();
            }

            var idsToSelect = numIds
                .Select(id => id.Value);

            var items = await _catalogContext.CatalogItems.Where(ci => idsToSelect.Contains(ci.Id)).ToListAsync();

            items = ChangeUriPlaceholder(items);

            return items;
        }

        private List<CatalogItem> ChangeUriPlaceholder(List<CatalogItem> items)
        {
            var baseUri = _settings.PicBaseUrl;
            var azureStorageEnabled = _settings.AzureStorageEnabled;

            foreach (var item in items)
            {
                item.FillProductUrl(baseUri, azureStorageEnabled: azureStorageEnabled);
            }

            return items;
        }
    }
}
