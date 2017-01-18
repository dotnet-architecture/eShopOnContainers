namespace Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel.BuyerAggregate
{
    using Microsoft.eShopOnContainers.Services.Ordering.Domain.SeedWork;
    using System;
    using System.Collections.Generic;

    public class Buyer
       :Entity,IAggregateRoot
    {
        public string FullName { get; private set; }

        public HashSet<Payment> Payments { get; private set; }

        protected Buyer() { }

        public Buyer(string fullName)
        {
            if (String.IsNullOrWhiteSpace(fullName))
            {
                throw new ArgumentNullException(nameof(fullName));
            }

            this.FullName = fullName;
            this.Payments = new HashSet<Payment>();
        }
    }
}
