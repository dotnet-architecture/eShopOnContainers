namespace Microsoft.eShopOnContainers.Services.Identity.API.Models.ManageViewModels
{
    public record ConfigureTwoFactorViewModel
    {
        public string SelectedProvider { get; init; }

        public ICollection<SelectListItem> Providers { get; init; }
    }
}
