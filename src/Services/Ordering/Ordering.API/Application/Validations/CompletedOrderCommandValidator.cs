namespace Microsoft.eShopOnContainers.Services.Ordering.API.Application.Validations;

public class CompletedOrderCommandValidator : AbstractValidator<SetCompletedOrderStatusCommand>
{
    public CompletedOrderCommandValidator(ILogger<CompletedOrderCommandValidator> logger)
    {
        RuleFor(order => order.OrderNumber).NotEmpty().WithMessage("No orderId found");      

        logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
    }
}
