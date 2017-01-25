using Microsoft.eShopOnContainers.Services.Ordering.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel.BuyerAggregate
{
    public class Buyer
      : Entity, IAggregateRoot
    {
        public string FullName { get; private set; }

        private HashSet<Payment> _payments;

        public IEnumerable<Payment> Payments => _payments?.ToList().AsEnumerable();

        protected Buyer() { }

        public Buyer(string identity)
        {
            if (String.IsNullOrWhiteSpace(identity))
            {
                throw new ArgumentNullException(nameof(identity));
            }

            FullName = identity;

            _payments = new HashSet<Payment>();
        }

        public Payment AddPayment(int cardTypeId, string alias, string cardNumber, string securityNumber, string cardHolderName, DateTime expiration)
        {
            var existingPayment = Payments.Where(p => p.IsEqualTo(cardTypeId, cardNumber, expiration))
                .SingleOrDefault();

            if (existingPayment != null)
            {
                return existingPayment;
            }
            else
            {
                var payment = new Payment(cardTypeId, alias, cardNumber, securityNumber, cardHolderName, expiration);

                _payments.Add(payment);

                return payment;
            }
        }
    }
}
