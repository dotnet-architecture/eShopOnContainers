namespace Microsoft.eShopOnContainers.Services.Ordering.Application.Commands
{
    using MediatR;

    public class NewOrderRequest
        :IAsyncRequest<bool>
    {
        public NewOrderRequest()
        {
        }
    }
}
