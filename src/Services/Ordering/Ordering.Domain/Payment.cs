namespace Microsoft.eShopOnContainers.Services.Ordering.Domain
{
    using Microsoft.eShopOnContainers.Services.Ordering.Domain.SeedWork;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;


    public class Payment
        : Entity, IAggregateRoot
    {
        public string CardNumber { get; private set; }

        public string SecurityNumber { get; private set; }

        public string CardHolderName { get; private set; }
    
        public int CardTypeId { get; private set; }

        public CardType CardType { get; private set; }

        public DateTime Expiration { get; private set; }

        protected Payment() { }
    }
}
