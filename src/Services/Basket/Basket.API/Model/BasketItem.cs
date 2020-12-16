using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Microsoft.eShopOnContainers.Services.Basket.API.Model
{
    public record BasketItem : IValidatableObject
    {
        public string Id { get; init; }
        public int ProductId { get; init; }
        public string ProductName { get; init; }
        public decimal UnitPrice { get; set; }
        public decimal OldUnitPrice { get; set; }
        public int Quantity { get; init; }
        public string PictureUrl { get; init; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            if (Quantity < 1)
            {
                results.Add(new ValidationResult("Invalid number of units", new []{ "Quantity" }));
            }

            return results;
        }
    }
}
