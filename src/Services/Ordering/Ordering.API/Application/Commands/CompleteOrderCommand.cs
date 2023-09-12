namespace Microsoft.eShopOnContainers.Services.Ordering.API.Application.Commands
{
    public class CompleteOrderCommand : IRequest<bool>
    {
        [DataMember]
        public int OrderNo { get; private set; }

        public CompleteOrderCommand(int orderNo)
        {
            this.OrderNo = orderNo;
        }
    }
}
