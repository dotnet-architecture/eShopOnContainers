using System;
using System.Collections.Generic;

using Microsoft.eShopOnContainers.Services.Ordering.Domain.SeedWork;

namespace Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel
{
    public class Address : ValueObject<Address> //A VO doesn't have IDENTITY, like in this case, the Address
    {
        public virtual String Street
        {
            get;
            private set;
        }

        public virtual String City
        {
            get;
            private set;
        }

        public virtual String State
        {
            get;
            private set;
        }

        public virtual String StateCode
        {
            get;
            private set;
        }

        public virtual String Country
        {
            get;
            private set;
        }

        public virtual String CountryCode
        {
            get;
            private set;
        }

        public virtual String ZipCode
        {
            get;
            private set;
        }

        public virtual double Latitude
        {
            get;
            private set;
        }

        public virtual double Longitude
        {
            get;
            private set;
        }
    }
}
