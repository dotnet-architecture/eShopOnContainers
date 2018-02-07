using FluentValidation;
using Ordering.API.Application.Commands;

namespace Ordering.API.Application.Validations
{
    public class ShipOrderCommandValidator
        : AbstractValidator<ShipOrderCommand>
    {
        public ShipOrderCommandValidator()
        {
            RuleFor(order => order.OrderNumber).NotEmpty().WithMessage("No orderId found");
        }
    }
}
