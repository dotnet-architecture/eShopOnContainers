namespace Microsoft.eShopOnContainers.Services.Ordering.API.Application.Validations
{
    public class CompleteOrderCommandValidator : AbstractValidator<CompleteOrderCommand>
    {
        public CompleteOrderCommandValidator(ILogger<CompleteOrderCommand> logger)
        {
            RuleFor(order => order.OrderNo).NotEmpty().WithMessage("No orderId found");

            logger.LogTrace("INSTANCE CREATED - {ClassName}", GetType().Name);
        }
    }
}
