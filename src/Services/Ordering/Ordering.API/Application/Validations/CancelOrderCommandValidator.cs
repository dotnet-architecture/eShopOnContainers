using FluentValidation;
using Ordering.API.Application.Commands;

namespace Ordering.API.Application.Validations
{
    public class CancelOrderCommandValidator
        : AbstractValidator<CancelOrderCommand>
    {
        public CancelOrderCommandValidator()
        {
            RuleFor(order => order.OrderNumber).NotEmpty().WithMessage("No orderId found");
        }
    }
}
