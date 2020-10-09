using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Catalog.API.Model;
using MediatR;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
using Microsoft.eShopOnContainers.Services.Catalog.API.Infrastructure.Commands;
using Microsoft.eShopOnContainers.Services.Catalog.API.IntegrationEvents.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.eShopOnContainers.Services.Catalog.API.Infrastructure.Services
{
    public class AzureStorage : IStorage
    {
        private readonly IMediator _mediator;
        private readonly ILogger<AzureStorage> _logger;
        private readonly IIdentityService _identityService;
        private readonly IEventBus _eventBus;
        private readonly IConfiguration _configuration;
        private readonly string azureStorageConnectionString;
        private readonly string azureStorageContainerName;
        public AzureStorage(IMediator mediator,
                            ILogger<AzureStorage> logger,
                            IIdentityService identityService,
                            IEventBus eventBus,
                            IConfiguration configuration)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger;
            _identityService = identityService;
            _eventBus = eventBus;
            _configuration = configuration;
            azureStorageConnectionString = _configuration["AzureStorageConnectionString"];
            azureStorageContainerName = _configuration["AzureStorageContainerName"];
        }

        public async Task InitAsync()
        {
            BlobContainerClient container = new BlobContainerClient(azureStorageConnectionString, azureStorageContainerName);
            await container.CreateIfNotExistsAsync();
        }
        public async Task SaveAsync(Payload payload)
        {
            string fileName = $"{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}{Path.GetExtension(payload.File.FileName)}";
            BlobContainerClient container = new BlobContainerClient(azureStorageConnectionString, azureStorageContainerName);
            BlobClient blob = container.GetBlobClient(fileName);
            try
            {
                await blob.UploadAsync(payload.File.OpenReadStream());
                if (!await _mediator.Send(new UpdatePicCommand(fileName, payload.CatalogItemId)))
                    await blob.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
            }
            catch (Exception ex)
            {
                _logger.LogError($"The uploading process has failed to request No. {payload.CatalogItemId} - Ex : {ex.Message}");
                _eventBus.Publish(new UploadingFailedIntegrationEvent(_identityService.GetUserIdentity(),
                                                                        $"The uploading process has failed to request No. {payload.CatalogItemId}"));
            }
        }
    }
}
