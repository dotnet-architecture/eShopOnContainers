namespace Microsoft.eShopOnContainers.Services.Ordering.Application.Commands
{
    using System;
    using MediatR;
    using Domain;
    using System.Collections;
    using System.Collections.Generic;

    public class NewOrderRequest
        :IAsyncRequest<bool>
    {

        private readonly List<OrderItem> _orderItems;
        public string City { get; set; }

        public string Street { get; set; }

        public string State { get; set; }

        public string Country { get; set; }

        public string ZipCode { get; set; }

        public string CardNumber { get; set; }

        public string CardHolderName { get; set; }

        public DateTime CardExpiration { get; set; }

        public string CardSecurityNumber { get; set; }

        public int CardTypeId { get; set; }

        public string Buyer { get; set; }

        public IEnumerable<OrderItem> OrderItems => _orderItems;

        public void AddOrderItem(OrderItem item)
        {
            _orderItems.Add(item);
        }

        public NewOrderRequest()
        {
            _orderItems = new List<OrderItem>();
        }
    }
}
