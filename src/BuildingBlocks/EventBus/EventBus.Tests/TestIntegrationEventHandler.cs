using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Tests
{
    public class TestIntegrationOtherEventHandler : IIntegrationEventHandler<TestIntegrationEvent>
    {
        public bool Handled { get; private set; }

        public TestIntegrationOtherEventHandler()
        {
            Handled = false;
        }

        public async Task Handle(TestIntegrationEvent @event)
        {
            Handled = true;
        }
    }
}
