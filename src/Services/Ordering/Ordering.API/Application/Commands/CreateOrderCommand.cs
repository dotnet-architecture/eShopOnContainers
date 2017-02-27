using System;
using MediatR;
using System.Collections.Generic;

namespace Microsoft.eShopOnContainers.Services.Ordering.API.Application.Commands
{
    

    public class CreateOrderCommand
        :IAsyncRequest<bool>
    {
        private readonly List<OrderItemDTO> _orderItems;

        public string City { get; private set; }

        public string Street { get; private set; }

        public string State { get; private set; }

        public string Country { get; private set; }

        public string ZipCode { get; private set; }

        public string CardNumber { get; private set; }

        public string CardHolderName { get; private set; }

        public DateTime CardExpiration { get; private set; }

        public string CardSecurityNumber { get; private set; }

        public int CardTypeId { get; private set; }

        public IEnumerable<OrderItemDTO> OrderItems => _orderItems;

        public void AddOrderItem(OrderItemDTO item)
        {
            _orderItems.Add(item);
        }

        public CreateOrderCommand()
        {
            _orderItems = new List<OrderItemDTO>();
        }

        public CreateOrderCommand(string city, string street, string state, string country, string zipcode, 
            string cardNumber, string cardHolderName, DateTime cardExpiration,
            string cardSecurityNumber, int cardTypeId) : this()
        {
            City = city;
            Street = street;
            State = state;
            Country = country;
            ZipCode = zipcode;
            CardNumber = cardNumber;
            CardHolderName = cardHolderName;
            CardSecurityNumber = cardSecurityNumber;
            CardTypeId = cardTypeId;
        }


        public class OrderItemDTO
        {
            public int ProductId { get; set; }

            public string ProductName { get; set; }

            public decimal UnitPrice { get; set; }

            public decimal Discount { get; set; }

            public int Units { get; set; }

            public string PictureUrl { get; set; }
        }
    }
}
