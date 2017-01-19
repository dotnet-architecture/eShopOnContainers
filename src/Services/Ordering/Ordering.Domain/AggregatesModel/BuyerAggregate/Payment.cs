namespace Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel.BuyerAggregate
{
    using Microsoft.eShopOnContainers.Services.Ordering.Domain.SeedWork;
    using System;


    public class Payment
        : Entity
    {
        public string Alias { get; private set; }
        public int BuyerId { get; private set; }

        public string CardNumber { get; private set; }

        public string SecurityNumber { get; private set; }

        public string CardHolderName { get; private set; }
    
        public int CardTypeId { get; private set; }

        public CardType CardType { get; private set; }

        public DateTime Expiration { get; private set; }

        protected Payment() { }

        public Payment(string alias, string cardNumber, string securityNumber, string cardHolderName, DateTime expiration, int cardTypeId)
        {
            if (String.IsNullOrWhiteSpace(cardNumber))
            {
                throw new ArgumentException(nameof(cardNumber));
            }

            if (String.IsNullOrWhiteSpace(securityNumber))
            {
                throw new ArgumentException(nameof(securityNumber));
            }
            
            if (String.IsNullOrWhiteSpace(cardHolderName))
            {
                throw new ArgumentException(nameof(cardHolderName));
            }

            if (expiration < DateTime.UtcNow)
            {
                throw new ArgumentException(nameof(expiration));
            }

            this.Alias = alias;
            this.CardNumber = cardNumber;
            this.SecurityNumber = securityNumber;
            this.CardHolderName = cardHolderName;
            this.Expiration = expiration;
            this.CardTypeId = cardTypeId;
        }
    }
}
