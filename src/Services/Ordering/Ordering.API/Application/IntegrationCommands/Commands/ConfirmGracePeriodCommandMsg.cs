using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ordering.API.Application.IntegrationCommands.Commands
{
    public class ConfirmGracePeriodCommandMsg : IntegrationEvent
    {
        public int OrderNumber { get; private set; }

        //TODO: message should change to Integration command type once command bus is implemented 
    }
}
