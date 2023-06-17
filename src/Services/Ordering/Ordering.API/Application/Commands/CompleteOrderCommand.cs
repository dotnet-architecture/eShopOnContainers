namespace Microsoft.eShopOnContainers.Services.Ordering.API.Application.Commands;

public class CompleteOrderCommand : IRequest<bool>
{

    [DataMember]
    public int OrderNumber { get; private set; }

    public CompleteOrderCommand(int orderNumber)
    {
        OrderNumber = orderNumber;
    }
}
