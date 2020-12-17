using System.Collections.Generic;

namespace Microsoft.eShopOnContainers.Services.Identity.API.Models.AccountViewModels
{
    public record ConsentInputModel
    {
        public string Button { get; init; }
        public IEnumerable<string> ScopesConsented { get; init; }
        public bool RememberConsent { get; init; }
        public string ReturnUrl { get; init; }
    }
}