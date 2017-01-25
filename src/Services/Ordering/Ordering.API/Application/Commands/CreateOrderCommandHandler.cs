namespace Microsoft.eShopOnContainers.Services.Ordering.API.Application.Commands
{
    using Domain.AggregatesModel.BuyerAggregate;
    using Domain.AggregatesModel.OrderAggregate;
    using MediatR;
    using System;
    using System.Threading.Tasks;

    public class CreateOrderCommandHandler
        : IAsyncRequestHandler<CreateOrderCommand, bool>
    {
        private readonly IBuyerRepository _buyerRepository;
        private readonly IOrderRepository _orderRepository;

        public CreateOrderCommandHandler(IBuyerRepository buyerRepository, IOrderRepository orderRepository)
        {
            if (buyerRepository == null)
            {
                throw new ArgumentNullException(nameof(buyerRepository));
            }

            if (orderRepository == null)
            {
                throw new ArgumentNullException(nameof(orderRepository));
            }

            _buyerRepository = buyerRepository;
            _orderRepository = orderRepository;
        }

        public async Task<bool> Handle(CreateOrderCommand message)
        {
            //find buyer/payment or add a new one buyer/payment 

            var buyer = await _buyerRepository.FindAsync(message.BuyerFullName);

            if (buyer == null)
            {
                buyer = new Buyer(message.BuyerFullName);
            }

            var payment = buyer.AddPayment(message.CardTypeId,
                $"Payment Method on {DateTime.UtcNow}",
                message.CardNumber,
                message.CardSecurityNumber,
                message.CardHolderName,
                message.CardExpiration);

            _buyerRepository.Add(buyer);

            await _buyerRepository.UnitOfWork
                .SaveChangesAsync();

            //create order for buyer and payment method

            var order = new Order(buyer.Id, payment.Id, new Address(message.Street, message.City, message.State, message.Country, message.ZipCode));

            foreach (var item in message.Items)
            {
                order.AddOrderItem(item.ProductId, item.ProductName, item.UnitPrice, item.Discount, item.Units);
            }

            _orderRepository.Add(order);

            var result = await _orderRepository.UnitOfWork
                .SaveChangesAsync();

            return result > 0;
        }
    }
}
