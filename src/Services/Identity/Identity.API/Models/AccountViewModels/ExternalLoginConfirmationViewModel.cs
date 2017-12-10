using System.ComponentModel.DataAnnotations;

namespace Microsoft.eShopOnContainers.Services.Identity.API.Models.AccountViewModels
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
