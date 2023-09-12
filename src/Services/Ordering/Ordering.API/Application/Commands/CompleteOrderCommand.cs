namespace Microsoft.eShopOnContainers.Services.Ordering.API.Application.Commands;

public class CompleteOrderCommand : IRequest<bool>
{

    [DataMember]
    public int OrderNumber { get; set; }
    public CompleteOrderCommand()
    {

    }
    public CompleteOrderCommand(int orderNumber)
    {
        OrderNumber = orderNumber;
    }
}
