namespace Microsoft.eShopOnContainers.Services.Ordering.Domain
{
    using Microsoft.eShopOnContainers.Services.Ordering.Domain.SeedWork;

    public class Buyer
       :Entity,IAggregateRoot
    {
        public string FullName { get; private set; }

        protected Buyer() { }
    }
}
