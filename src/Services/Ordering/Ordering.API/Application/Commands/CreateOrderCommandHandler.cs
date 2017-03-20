namespace Microsoft.eShopOnContainers.Services.Ordering.API.Application.Commands
{
    using Domain.AggregatesModel.BuyerAggregate;
    using Domain.AggregatesModel.OrderAggregate;
    using MediatR;
    using Microsoft.eShopOnContainers.Services.Ordering.API.Infrastructure.Services;
    using Microsoft.eShopOnContainers.Services.Ordering.Infrastructure.Repositories;
    using System;
    using System.Threading.Tasks;


    public class CreateOrderCommandIdentifiedHandler : IdentifierCommandHandler<CreateOrderCommand, bool>
    {
        public CreateOrderCommandIdentifiedHandler(IMediator mediator, IRequestManager requestManager) : base(mediator, requestManager)
        {
        }

        protected override bool CreateResultForDuplicateRequest()
        {
            return true;                // Ignore duplicate requests for creating order.
        }
    }

    public class CreateOrderCommandHandler
        : IAsyncRequestHandler<CreateOrderCommand, bool>
    {
        private readonly IBuyerRepository _buyerRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IIdentityService _identityService;

        // Using DI to inject infrastructure persistence Repositories
        public CreateOrderCommandHandler(IBuyerRepository buyerRepository, IOrderRepository orderRepository, IIdentityService identityService)
        {
            _buyerRepository = buyerRepository ?? throw new ArgumentNullException(nameof(buyerRepository));
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
        }

        public async Task<bool> Handle(CreateOrderCommand message)
        {
            // Add/Update the Buyer AggregateRoot
            // DDD patterns comment: Add child entities and value-objects through the Order Aggregate-Root
            // methods and constructor so validations, invariants and business logic 
            // make sure that consistency is preserved across the whole aggregate

            var cardTypeId = message.CardTypeId != 0 ? message.CardTypeId : 1;

            var buyerGuid = _identityService.GetUserIdentity();
            var buyer = await _buyerRepository.FindAsync(buyerGuid);

            if (buyer == null)
            {
                buyer = new Buyer(buyerGuid);
            }

            var payment = buyer.AddPaymentMethod(cardTypeId,
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
