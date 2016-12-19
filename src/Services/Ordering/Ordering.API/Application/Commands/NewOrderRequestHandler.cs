namespace Microsoft.eShopOnContainers.Services.Ordering.Api.Application.Commands
{
    using Domain.Repositories;
    using MediatR;
    using System.Linq;
    using System;
    using System.Threading.Tasks;
    using Domain;

    public class NewOrderRequestHandler
        : IAsyncRequestHandler<NewOrderRequest, bool>
    {
        private readonly IBuyerRepository _buyerRepository;
        private readonly IOrderRepository _orderRepository;

        public NewOrderRequestHandler(IBuyerRepository buyerRepository,IOrderRepository orderRepository)
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
        public async Task<bool> Handle(NewOrderRequest message)
        {
            //find buyer/payment or add a new one buyer/payment 

            var buyer = await _buyerRepository.FindAsync(message.Buyer);

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



        Payment GetExistingPaymentOrAddANewOne(Buyer buyer, NewOrderRequest message)
        {
            Payment payment = PaymentAlreadyExist(buyer, message);

            if (payment == null)
            {
                payment = CreatePayment(message);
                buyer.Payments.Add(payment);
            }

            return payment;

        }

        Payment PaymentAlreadyExist(Domain.Buyer buyer, NewOrderRequest message)
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

        Buyer CreateBuyer(NewOrderRequest message)
        {
            return _buyerRepository.Add(
                new Buyer(message.Buyer));
        }

        Order CreateOrder(int buyerId, int paymentId, int addressId)
        {
            return new Order(buyerId, paymentId);
        }

        Payment CreatePayment(NewOrderRequest message)
        {
            return new Payment(message.CardNumber, message.CardSecurityNumber, message.CardHolderName, message.CardExpiration, message.CardTypeId);
        }
    }
}
