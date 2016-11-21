namespace Microsoft.eShopOnContainers.Services.Ordering.Application.Commands
{
    using MediatR;
    using System;
    using System.Threading.Tasks;

    public class NewOrderREquestHandler
        : IAsyncRequestHandler<NewOrderRequest, bool>
    {
        public Task<bool> Handle(NewOrderRequest message)
        {
            throw new NotImplementedException();
        }
    }
}
