namespace Microsoft.eShopOnContainers.Services.Ordering.Application.Commands
{
    using MediatR;

    public class CancelOrderRequest
        : IAsyncRequest<bool>
    {
        public int OrderId { get; private set; }

        public CancelOrderRequest(int orderId)
        {
            OrderId = orderId;
        }
    }
}
