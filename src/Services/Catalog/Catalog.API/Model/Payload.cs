using Microsoft.AspNetCore.Http;

namespace Catalog.API.Model
{
    public class Payload
    {
        public Payload(IFormFile file, int catalogItemId)
        {
            File = file;
            CatalogItemId = catalogItemId;
        }
        public IFormFile File { get; private set; }
        public int CatalogItemId { get; private set; }
    }
}
