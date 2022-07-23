namespace Microsoft.eShopOnContainers.Services.Ordering.API.Application.Commands;

public class SetCompletedOrderStatusCommand : IRequest<bool>
{

    [DataMember]
    public int OrderNumber { get; private set; }

    public SetCompletedOrderStatusCommand(int orderNumber)
    {
        OrderNumber = orderNumber;
    }
}
