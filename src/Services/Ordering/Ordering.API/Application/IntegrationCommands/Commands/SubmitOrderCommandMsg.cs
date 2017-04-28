using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;

namespace Ordering.API.Application.IntegrationCommands.Commands
{
    public class SubmitOrderCommandMsg : IntegrationEvent
    {
        public int OrderNumber { get; private set; }
        //TODO: message should change to Integration command type once command bus is implemented 
    }
}
