using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Ordering.API.Application.Commands
{
    public class SetStockRejectedOrderStatusCommand : IRequest<bool>
    {

        [DataMember]
        public int OrderNumber { get; private set; }

        [DataMember]
        public List<int> OrderStockItems { get; private set; }

        public SetStockRejectedOrderStatusCommand(int orderNumber, List<int> orderStockItems)
        {
            OrderNumber = orderNumber;
            OrderStockItems = orderStockItems;
        }
    }
}