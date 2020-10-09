using Microsoft.AspNetCore.Http;
using Microsoft.eShopOnContainers.Services.Catalog.API.Infrastructure.Services;
using System.IO;
using System.Linq;

namespace Microsoft.eShopOnContainers.Services.Catalog.API.Infrastructure.Validators
{
    public class IsFileExtntionSuitable : ISpecification<IFormFile>
    {
        public bool IsSatisfiedBy(IFormFile file)
        {
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            string[] PermittedExtensions = { ".jpg", ".jpeg", ".png" };
            return !string.IsNullOrEmpty(ext) && PermittedExtensions.Contains(ext);
        }
    }
}
