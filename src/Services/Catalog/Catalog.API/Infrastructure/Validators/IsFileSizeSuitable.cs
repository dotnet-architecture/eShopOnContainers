using Microsoft.AspNetCore.Http;
using Microsoft.eShopOnContainers.Services.Catalog.API.Infrastructure.Services;
using Microsoft.Extensions.Options;

namespace Microsoft.eShopOnContainers.Services.Catalog.API.Infrastructure.Validators
{
    public class IsFileSizeSuitable : ISpecification<IFormFile>
    {
        private readonly IOptions<CatalogSettings> options;

        public IsFileSizeSuitable(IOptions<CatalogSettings> options)
        {
            this.options = options;
        }
        public bool IsSatisfiedBy(IFormFile file) =>  options.Value.FileSizeLimit >= file.Length;
    }
}
