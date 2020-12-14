using System;

namespace Basket.API.Model
{
    public record BasketCheckout
    {
        public string City { get; init; }

        public string Street { get; init; }

        public string State { get; init; }

        public string Country { get; init; }

        public string ZipCode { get; init; }

        public string CardNumber { get; init; }

        public string CardHolderName { get; init; }

        public DateTime CardExpiration { get; init; }

        public string CardSecurityNumber { get; init; }

        public int CardTypeId { get; init; }

        public string Buyer { get; init; }

        public Guid RequestId { get; set; }
    }
}

