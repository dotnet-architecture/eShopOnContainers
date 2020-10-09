using Microsoft.AspNetCore.Http;
using Microsoft.eShopOnContainers.Services.Catalog.API.Infrastructure.Services;

namespace Microsoft.eShopOnContainers.Services.Catalog.API.Infrastructure.Validators
{
    public class IsFileNotNull : ISpecification<IFormFile>
    {
        public bool IsSatisfiedBy(IFormFile file) => file?.Length > 0;
    }
}
