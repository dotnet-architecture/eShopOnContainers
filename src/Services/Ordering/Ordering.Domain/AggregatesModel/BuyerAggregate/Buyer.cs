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

        public Buyer(string IdentityGuid)
        {
            if (String.IsNullOrWhiteSpace(IdentityGuid))
            {
                throw new ArgumentNullException(nameof(IdentityGuid));
            }

            this.FullName = IdentityGuid;
            this.Payments = new HashSet<Payment>();
        }
    }
}
