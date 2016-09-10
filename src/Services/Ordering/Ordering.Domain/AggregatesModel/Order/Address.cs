using System;
using System.Collections.Generic;

using Microsoft.eShopOnContainers.Services.Ordering.Domain.SeedWork;

namespace Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel
{
    public class Address : ValueObject<Address> 
    {
        public Address(string street, 
                       string city, 
                       string state, 
                       string stateCode, 
                       string country, 
                       string countryCode, 
                       string zipCode,
                       double latitude = 0,
                       double longitude = 0
                       )
        {
            if (street == null)
                throw new ArgumentNullException("street");

            if (city == null)
                throw new ArgumentNullException("city");

            if (state == null)
                throw new ArgumentNullException("state");

            if (stateCode == null)
                throw new ArgumentNullException("stateCode");

            if (country == null)
                throw new ArgumentNullException("country");

            if (countryCode == null)
                throw new ArgumentNullException("countryCode");

            if (zipCode == null)
                throw new ArgumentNullException("zipCode");

            //Generate the ID guid - Remove this when EF Core supports ValueObjects
            // https://github.com/aspnet/EntityFramework/issues/246
            this.Id = Guid.NewGuid();

            this.Street = street;
            this.City = city;
            this.State = state;
            this.StateCode = stateCode;
            this.Country = country;
            this.CountryCode = countryCode;
            this.ZipCode = zipCode;
            this.Latitude = latitude;
            this.Longitude = longitude;
        }

        //Infrastructure requisite - Parameterless constructor needed by EF
        Address() { }

        public virtual String Street { get; private set; }      
        public virtual String City { get; private set; }       
        public virtual String State { get; private set; }       
        public virtual String StateCode { get; private set; }
        public virtual String Country { get; private set; }
        public virtual String CountryCode { get; private set; }
        public virtual String ZipCode { get; private set; }
        public virtual double Latitude { get; private set; }
        public virtual double Longitude { get; private set; }
    }
}
