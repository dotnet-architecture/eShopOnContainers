namespace Payment.API.IntegrationCommands.CommandHandlers
{
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
    using Payment.API.IntegrationCommands.Commands;
    using System.Threading.Tasks;
    using System;

    public class PayOrderCommandMsgHandler : IIntegrationEventHandler<PayOrderCommandMsg>
    {
        public PayOrderCommandMsgHandler()
        {
        }

        public async Task Handle(PayOrderCommandMsg @event)
        {
            throw new NotImplementedException();
        }
    }
}
