using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.eShopOnContainers.Services.Catalog.API.Infrastructure.Commands
{
    public class UpdatePicCommandHandler
        : IRequestHandler<UpdatePicCommand, bool>
    {
        private readonly CatalogContext _context;
        private readonly ILogger<UpdatePicCommandHandler> _logger;

        public UpdatePicCommandHandler(CatalogContext context, ILogger<UpdatePicCommandHandler> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context)); ;
            _logger = logger;
        }
        public async Task<bool> Handle(UpdatePicCommand command, CancellationToken cancellationToken)
        {
            var item = await _context.CatalogItems.FirstOrDefaultAsync(a => a.Id == command.CatalogItemId);
            if (item == null) return false;
            item.PictureFileName = command.PictureFileName;
            _context.CatalogItems.Update(item);
            _logger.LogInformation($"Starting Updating Pic info for Catalog Item Id No: {command.CatalogItemId} - Command: {command}");
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
