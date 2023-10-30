namespace Microsoft.eShopOnContainers.Services.Ordering.API.Application.Validations
{
    public class CompleteOrderCommandValidator : AbstractValidator<CompleteOrderCommand>
    {
        public CompleteOrderCommandValidator(ILogger<CompleteOrderCommandValidator> logger)
        {
            RuleFor(command => command.OrderId)
                .GreaterThan(0)
                .WithMessage("OrderId should be a positive number.");

            logger.LogTrace("INSTANCE CREATED - {ClassName}", GetType().Name);
        }
    }
}
