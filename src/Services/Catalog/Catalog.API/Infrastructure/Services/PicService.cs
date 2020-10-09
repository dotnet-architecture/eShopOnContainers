using Catalog.API.Model;
using System;

namespace Microsoft.eShopOnContainers.Services.Catalog.API.Infrastructure.Services
{
    public class PicService : IPicService
    {        
        public event EventHandler<Payload> OnFileUploaded;
        public void UploadFile(Payload payload) => OnFileUploaded?.Invoke(this, payload);
    }
}
