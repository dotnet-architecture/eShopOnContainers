using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Ordering.API.Application.Commands
{
    public class SetStockConfirmedOrderStatusCommand : IRequest<bool>
    {

        [DataMember]
        public int OrderNumber { get; private set; }

        public SetStockConfirmedOrderStatusCommand(int orderNumber)
        {
            OrderNumber = orderNumber;
        }
    }
}