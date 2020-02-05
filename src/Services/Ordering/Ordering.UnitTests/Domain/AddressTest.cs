using Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel.OrderAggregate;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Ordering.UnitTests.Domain
{
    public class AddressTest
    {
        [Fact]
        public void equal_addresses_should_be_equal()
        {
            // Arrange
            var left = CreateValidAddress();
            var right = CreateValidAddress();

            // Act and Assert
            Assert.Equal(left, right);
            Assert.True(left == right);
        }

        private Address CreateValidAddress() => new Address(
                street: "FakeStreet",
                city: "FakeCity",
                state: "FakeState",
                country: "FakeCountry",
                zipcode: "FakeZipCode");
    }
}
