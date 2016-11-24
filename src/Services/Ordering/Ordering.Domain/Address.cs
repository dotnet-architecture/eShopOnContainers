namespace Microsoft.eShopOnContainers.Services.Ordering.Domain
{
    using Microsoft.eShopOnContainers.Services.Ordering.Domain.SeedWork;
    using System;

    public class Address
        : Entity
    {
        public String Street { get; set; }

        public String City { get; set; }

        public String State { get; set; }

        public String Country { get; set; }

        public String ZipCode { get; set; }

    }
}
