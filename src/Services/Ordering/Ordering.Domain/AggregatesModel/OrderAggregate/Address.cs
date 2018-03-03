using Microsoft.eShopOnContainers.Services.Ordering.Domain.SeedWork;
using System;
using System.Collections.Generic;

namespace Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel.OrderAggregate
{
    public sealed class Address : ValueObject
    {
        private String Street;
        private String City;
        private String State;
        private String Country;
        private String ZipCode;

        public static Address fromString(String address) 
        {
            Guards.AssertStringIsNotEmptyOrNull(address);
            Guards.AssertStringLowerBound(address,5);

            //Address toString() format with '_' between fields
            string[] addressElements = address.Split('_');
            Guards.AssertIsTrue(addressElements.Length == 5, "Invalid Address string format");

            //toString() order
            String Street = addressElements[0];
            String ZipCode = addressElements[1];
            String City = addressElements[2];
            String State = addressElements[3];
            String Country = addressElements[4];

            return new Address(Street, ZipCode, City, State, Country);
        }

        public Address(string street, string zipcode, string city, string state, string country)
        {
            Guards.AssertStringIsNotEmptyOrNull(street);
            Guards.AssertStringLowerBound(street,3);
            this.Street = street;

            Guards.AssertStringIsNotEmptyOrNull(zipcode);
            Guards.AssertStringLowerBound(zipcode, 3);
            this.ZipCode = zipcode;

            Guards.AssertStringIsNotEmptyOrNull(city);
            Guards.AssertStringLowerBound(city, 3);
            this.City = city;

            Guards.AssertStringIsNotEmptyOrNull(state);
            Guards.AssertStringLowerBound(state, 3);
            this.State = state;

            Guards.AssertStringIsNotEmptyOrNull(country);
            Guards.AssertStringLowerBound(country, 3);
            this.Country = country;

        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            // Using a yield return statement to return each element one at a time
            yield return Street;
            yield return City;
            yield return State;
            yield return Country;
            yield return ZipCode;
        }

        public override string ToString()
        => $"{Street}_{ZipCode}_{City}_{State}_({Country})";

    }
}
