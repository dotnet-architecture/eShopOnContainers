using FluentValidation;
using MediatR;
using Microsoft.eShopOnContainers.Services.Ordering.API.Application.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Microsoft.eShopOnContainers.Services.Ordering.API.Application.Commands.CreateOrderCommand;

namespace Ordering.API.Application.Validations
{
    public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderCommandValidator()
        {
            RuleFor(order => order.City).NotEmpty();
            RuleFor(order => order.Street).NotEmpty();
            RuleFor(order => order.State).NotEmpty();
            RuleFor(order => order.Country).NotEmpty();
            RuleFor(order => order.ZipCode).NotEmpty();
            RuleFor(order => order.CardNumber).NotEmpty().Length(12, 19); 
            RuleFor(order => order.CardHolderName).NotEmpty();
            RuleFor(order => order.CardExpiration).NotEmpty().Must(BeValidExpirationDate).WithMessage("Please specify a valid card expiration date"); 
            RuleFor(order => order.CardSecurityNumber).NotEmpty().Length(3); 
            RuleFor(order => order.CardTypeId).NotEmpty();
            RuleFor(order => order.OrderItems).Must(ContainOrderItems).WithMessage("No order items found"); 
        }

        private bool BeValidExpirationDate(DateTime dateTime)
        {
            return dateTime >= DateTime.UtcNow;
        }

        private bool ContainOrderItems(IEnumerable<OrderItemDTO> orderItems)
        {
            return orderItems.Any();
        }
    }
}
