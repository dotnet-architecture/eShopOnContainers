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
            _logger.LogInformation($"Begin grpc call CatalogService.GetItemById for product id {request.Id}");
            if (request.Id <=0)
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
    }
}
