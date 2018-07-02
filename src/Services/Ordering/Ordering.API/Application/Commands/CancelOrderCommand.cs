using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Microsoft.eShopOnContainers.Services.Ordering.API.Application.Commands;

namespace Ordering.API.Application.Commands
{
    public class CancelOrderCommand : ICommand<bool>
    {
        public Guid CommandId { get; set; }

        [DataMember]
        public int OrderNumber { get; private set; }

        public CancelOrderCommand(int orderNumber)
        {
            OrderNumber = orderNumber;
        }
    }
}
