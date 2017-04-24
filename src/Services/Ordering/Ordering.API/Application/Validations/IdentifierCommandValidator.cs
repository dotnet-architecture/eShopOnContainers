using FluentValidation;
using Microsoft.eShopOnContainers.Services.Ordering.API.Application.Commands;

namespace Ordering.API.Application.Validations
{
    public class IdentifierCommandValidator : AbstractValidator<IdentifiedCommand<CreateOrderCommand,bool>>
    {
        public IdentifierCommandValidator()
        {
            RuleFor(command => command.Id).NotEmpty();    
        }
    }
}
