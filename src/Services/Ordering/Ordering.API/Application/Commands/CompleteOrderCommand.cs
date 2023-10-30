namespace Microsoft.eShopOnContainers.Services.Ordering.API.Application.Commands
{
    public class CompleteOrderCommand : IRequest<CompleteOrderDTO>
    {
        public int OrderId { get; set; }
        public CompleteOrderCommand(int orderNumber)
        {
            OrderId = orderNumber;
        }

    }
}
