namespace Microsoft.eShopOnContainers.Services.Ordering.API.Application.Validations;

public class CompleteOrderCommandValidator : AbstractValidator<CompleteOrderCommand>
{
    public CompleteOrderCommandValidator(ILogger<CompleteOrderCommandValidator> logger)
    {
        RuleFor(order => order.OrderNumber).NotEmpty().WithMessage("No orderId found");

        logger.LogTrace("INSTANCE CREATED - {ClassName}", GetType().Name);
    }
}
