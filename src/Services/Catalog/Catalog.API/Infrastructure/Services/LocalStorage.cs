using Catalog.API.Model;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
using Microsoft.eShopOnContainers.Services.Catalog.API.Infrastructure.Commands;
using Microsoft.eShopOnContainers.Services.Catalog.API.IntegrationEvents.Events;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.eShopOnContainers.Services.Catalog.API.Infrastructure.Services
{
    public class LocalStorage : IStorage
    {
        private readonly IWebHostEnvironment _env;        
        private readonly IMediator _mediator;
        private readonly ILogger<LocalStorage> _logger;
        private readonly IIdentityService _identityService;
        private readonly IEventBus _eventBus;

        public LocalStorage(IWebHostEnvironment env,
                            IMediator mediator,
                            ILogger<LocalStorage> logger,
                            IIdentityService identityService,
                            IEventBus eventBus)
        {
            _env = env;                        
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger;
            _identityService = identityService;
            _eventBus = eventBus;
        }
        public async Task SaveAsync(Payload payload)
        {
            string fileName = $"{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}{Path.GetExtension(payload.File.FileName)}";
            var filePath = Path.Combine(_env.WebRootPath, fileName);
            try
            {
                using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    await payload.File.CopyToAsync(stream);
                }
                if(!await _mediator.Send(new UpdatePicCommand(fileName, payload.CatalogItemId)))
                    File.Delete(filePath);
            }
            catch (Exception ex)
            {
                _logger.LogError($"The uploading process has failed to request No. {payload.CatalogItemId} - Ex : {ex.Message}");
                _eventBus.Publish(new UploadingFailedIntegrationEvent(_identityService.GetUserIdentity(),
                                                                        $"The uploading process has failed, Item ID: {payload.CatalogItemId}"));
            }
        }
    }
}
