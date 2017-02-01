using Microsoft.eShopOnContainers.Services.Ordering.Domain.Seedwork;
using System;

namespace Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel.BuyerAggregate
{
    public class PaymentMethod
        : Entity
    {
        private int _buyerId;
        private string _alias;
        private string _cardNumber;
        private string _securityNumber;
        private string _cardHolderName;
        private DateTime _expiration;

        private int _cardTypeId;
        public CardType CardType { get; private set; }


        protected PaymentMethod() { }

        public PaymentMethod(int cardTypeId, string alias, string cardNumber, string securityNumber, string cardHolderName, DateTime expiration)
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

            _alias = alias;
            _cardNumber = cardNumber;
            _securityNumber = securityNumber;
            _cardHolderName = cardHolderName;
            _expiration = expiration;
            _cardTypeId = cardTypeId;
        }

        public bool IsEqualTo(int cardTypeId, string cardNumber,DateTime expiration)
        {
            return _cardTypeId == cardTypeId
                && _cardNumber == cardNumber
                && _expiration == expiration;
        }
    }
}
