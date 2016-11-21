namespace Microsoft.eShopOnContainers.Services.Ordering.Application.Commands
{
    using MediatR;
    using System;
    using System.Threading.Tasks;

    public class CancelOrderRequestHandler
        : IAsyncRequestHandler<CancelOrderRequest, bool>
    {
        public Task<bool> Handle(CancelOrderRequest message)
        {
            throw new NotImplementedException();
        }
    }
}
