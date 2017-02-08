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

        // Using DI to inject infrastructure persistence Repositories
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
            // Add/Update the Buyer AggregateRoot
            // DDD patterns comment: Add child entities and value-objects through the Order Aggregate-Root
            // methods and constructor so validations, invariants and business logic 
            // make sure that consistency is preserved across the whole aggregate

            var buyer = await _buyerRepository.FindAsync(message.BuyerIdentityGuid);

            if (buyer == null)
            {
                buyer = new Buyer(message.BuyerIdentityGuid);
            }

            var payment = buyer.AddPaymentMethod(message.CardTypeId,
                $"Payment Method on {DateTime.UtcNow}",
                message.CardNumber,
                message.CardSecurityNumber,
                message.CardHolderName,
                message.CardExpiration);

            _buyerRepository.Add(buyer);

            await _buyerRepository.UnitOfWork
                .SaveChangesAsync();

            // Create the Order AggregateRoot
            // DDD patterns comment: Add child entities and value-objects through the Order Aggregate-Root
            // methods and constructor so validations, invariants and business logic 
            // make sure that consistency is preserved across the whole aggregate

            var order = new Order(buyer.Id, payment.Id, new Address(message.Street, message.City, message.State, message.Country, message.ZipCode));

            foreach (var item in message.OrderItems)
            {
                order.AddOrderItem(item.ProductId, item.ProductName, item.UnitPrice, item.Discount, item.PictureUrl, item.Units);
            }

            _orderRepository.Add(order);

            var result = await _orderRepository.UnitOfWork
                .SaveChangesAsync();

            return result > 0;

        }
    }
}
