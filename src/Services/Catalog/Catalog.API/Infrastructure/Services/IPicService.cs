using Catalog.API.Model;
using System;

namespace Microsoft.eShopOnContainers.Services.Catalog.API.Infrastructure.Services
{
    public interface IPicService
    {
        event EventHandler<Payload> OnFileUploaded;
        void UploadFile(Payload payload);
    }
}
