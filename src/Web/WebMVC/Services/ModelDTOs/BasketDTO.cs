using System;
using System.ComponentModel.DataAnnotations;

namespace WebMVC.Services.ModelDTOs
{
    public record BasketDTO
    {
        [Required]
        public string City { get; init; }
        [Required]
        public string Street { get; init; }
        [Required]
        public string State { get; init; }
        [Required]
        public string Country { get; init; }

        public string ZipCode { get; init; }
        [Required]
        public string CardNumber { get; init; }
        [Required]
        public string CardHolderName { get; init; }

        [Required]
        public DateTime CardExpiration { get; init; }

        [Required]
        public string CardSecurityNumber { get; init; }

        public int CardTypeId { get; init; }

        public string Buyer { get; init; }

        [Required]
        public Guid RequestId { get; init; }
    }
}

