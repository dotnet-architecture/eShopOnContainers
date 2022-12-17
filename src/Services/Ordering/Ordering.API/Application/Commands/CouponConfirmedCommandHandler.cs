namespace Ordering.API.Application.Commands;

public class CouponConfirmedCommandHandler : IRequestHandler<CouponConfirmedCommand, bool>
{
    private readonly IOrderRepository _orderRepository;

    public CouponConfirmedCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<bool> Handle(CouponConfirmedCommand command, CancellationToken cancellationToken)
    {
        var orderToUpdate = await _orderRepository.GetAsync(command.OrderNumber);

        if (orderToUpdate == null)
        {
            return false;
        }

        orderToUpdate.ProcessCouponConfirmed();

        return await _orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}

public class CouponConfirmIdenfifiedCommandHandler : IdentifiedCommandHandler<CouponConfirmedCommand, bool>
{
    public CouponConfirmIdenfifiedCommandHandler(
        IMediator mediator,
        IRequestManager requestManager,
        ILogger<IdentifiedCommandHandler<CouponConfirmedCommand, bool>> logger)
        : base(mediator, requestManager, logger)
    {
    }

    protected override bool CreateResultForDuplicateRequest()
    {
        return true;                // Ignore duplicate requests for processing order.
    }
}
