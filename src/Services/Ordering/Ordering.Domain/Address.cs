namespace Microsoft.eShopOnContainers.Services.Ordering.Domain
{
    using Microsoft.eShopOnContainers.Services.Ordering.Domain.SeedWork;
    using System;

    public class Address
        : Entity
    {
        public String Street { get; private set; }

        public String City { get; private set; }

        public String State { get; private set; }

        public String StateCode { get; private set; }

        public String Country { get; private set; }

        public String CountryCode { get; private set; }

        public String ZipCode { get; private set; }

        public double Latitude { get; private set; }

        public double Longitude { get; private set; }

        protected Address() { }
    }
}
