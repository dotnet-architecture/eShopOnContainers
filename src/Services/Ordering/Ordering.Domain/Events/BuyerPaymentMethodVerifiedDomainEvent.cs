using MediatR;
using Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel.BuyerAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ordering.Domain.Events
{
    public class BuyerAndPaymentMethodVerifiedDomainEvent
        : IAsyncNotification
    {
        public Buyer Buyer { get; private set; }
        public PaymentMethod Payment { get; private set; }
        public int OrderId { get; private set; }

        public BuyerAndPaymentMethodVerifiedDomainEvent(Buyer buyer, PaymentMethod payment, int orderId)
        {
            Buyer = buyer;
            Payment = payment;
            OrderId = orderId;
        }
    }
}
