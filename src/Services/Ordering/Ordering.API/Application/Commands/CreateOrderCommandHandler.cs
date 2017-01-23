namespace Microsoft.eShopOnContainers.Services.Ordering.Api.Application.Commands
{
    using Domain.AggregatesModel.OrderAggregate;
    using Domain.AggregatesModel.BuyerAggregate;
    using MediatR;
    using System.Linq;
    using System;
    using System.Threading.Tasks;
    using Domain;

    public class CreateOrderCommandHandler
        : IAsyncRequestHandler<CreateOrderCommand, bool>
    {
        private readonly IBuyerRepository _buyerRepository;
        private readonly IOrderRepository _orderRepository;

        public CreateOrderCommandHandler(IBuyerRepository buyerRepository,IOrderRepository orderRepository)
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

            var buyer = await _buyerRepository.FindAsync(message.BuyerIdentityGuid);

            if (buyer == null)
            {
                buyer = CreateBuyer(message);
            }

            var payment = GetExistingPaymentOrAddANewOne(buyer, message);

            await _buyerRepository.UnitOfWork
                .SaveChangesAsync();

            //create order for buyer and payment method

            var order = CreateOrder(buyer.Id, payment.Id, 0);
            order.SetAddress( new Address()
            {
                City = message.City,
                State = message.State,
                Street = message.Street,
                ZipCode = message.ZipCode
            });

            foreach (var item in message.OrderItems)
            {
                order.AddOrderItem(item);
            }

            _orderRepository.Add(order);

            var result = await _orderRepository.UnitOfWork
                .SaveChangesAsync();

            return result > 0;
        }

        Buyer CreateBuyer(CreateOrderCommand message)
        {
            return _buyerRepository.Add(
                new Buyer(message.BuyerIdentityGuid));
        }

        Order CreateOrder(int buyerId, int paymentId, int addressId)
        {
            return new Order(buyerId, paymentId);
        }


        //TO DO:
        //(CDLTLL) This is wrong. We shouldn't be able to create a PaymentMethod from a CommandHandler or anywhere in the Application Layer
        //because a PaymentMethod is a child-entity, part of the Buyer Aggregate.
        //So, any creation/update of a PaymentMethod should be done through its Aggregate-Root: the Buyer root entity.
        //Need to move this logic to the Buyer Aggregate-Root and rename to "AddPaymentMethod()"
        Payment CreatePayment(CreateOrderCommand message)
        {
            return new Payment("My Default Payment Method", message.CardNumber, message.CardSecurityNumber, message.CardHolderName, message.CardExpiration, message.CardTypeId);
        }

        //TO DO:
        //(CDLTLL) This is wrong. As explained, this logic should be part of the
        //Buyer Aggregate Root, as a PaymentMethod is a child-entity of that Aggregate.
        Payment GetExistingPaymentOrAddANewOne(Buyer buyer, CreateOrderCommand message)
        {
            Payment payment = PaymentAlreadyExist(buyer, message);

            if (payment == null)
            {
                payment = CreatePayment(message);
                buyer.Payments.Add(payment);
            }

            return payment;

        }

        //TO DO:
        //(CDLTLL) This is wrong. As explained, this logic should be part of the
        //Buyer Aggregate Root, as a PaymentMethod is a child-entity of that Aggregate.
        Payment PaymentAlreadyExist(Buyer buyer, CreateOrderCommand message)
        {
            return buyer.Payments
                .SingleOrDefault(p =>
                {
                    return p.CardHolderName == message.CardHolderName
                    &&
                    p.CardNumber == message.CardNumber
                    &&
                    p.Expiration == message.CardExpiration
                    &&
                    p.SecurityNumber == message.CardSecurityNumber;
                });
        }
    }
}
