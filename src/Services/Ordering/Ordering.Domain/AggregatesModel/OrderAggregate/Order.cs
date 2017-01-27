using Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel.BuyerAggregate;
using Microsoft.eShopOnContainers.Services.Ordering.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel.OrderAggregate
{
    public class Order
        : Entity
    {
        private string _street;
        private string _city;
        private string _state;
        private string _country;
        private string _zipCode;
        private DateTime _orderDate;

        public Buyer Buyer { get; private set; }
        int _buyerId;

        public OrderStatus OrderStatus { get; private set; }
        int _orderStatusId;

        HashSet<OrderItem> _orderItems;
        public IEnumerable<OrderItem> OrderItems => _orderItems.ToList().AsEnumerable();

        public PaymentMethod PaymentMethod { get; private set; }
        int _paymentMethodId;

        protected Order() { }

        public Order(int buyerId, int paymentMethodId, Address address)
        {

            _buyerId = buyerId;
            _paymentMethodId = paymentMethodId;
            _orderStatusId = OrderStatus.InProcess.Id;
            _orderDate = DateTime.UtcNow;
            _street = address.Street;
            _city = address.City;
            _state = address.State;
            _country = address.Country;
            _zipCode = address.ZipCode;

            _orderItems = new HashSet<OrderItem>();
        }


        public void AddOrderItem(int productId, string productName, decimal unitPrice, decimal discount, int units = 1)
        {
            var existingOrderForProduct = _orderItems.Where(o => o.ProductId == productId)
                .SingleOrDefault();

            if (existingOrderForProduct != null)
            {
                //if previous line exist modify it with higher discount  and units..

                if (discount > existingOrderForProduct.GetCurrentDiscount())
                {
                    existingOrderForProduct.SetNewDiscount(discount);
                    existingOrderForProduct.AddUnits(units);
                }
            }
            else
            {
                //add validated new order item

                var orderItem = new OrderItem(productId, productName, unitPrice, discount, units);

                _orderItems.Add(orderItem);
            }
        }
    }
}
