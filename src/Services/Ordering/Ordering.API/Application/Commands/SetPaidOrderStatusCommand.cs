using MediatR;
using System.Runtime.Serialization;

namespace Ordering.API.Application.Commands
{
    public class SetPaidOrderStatusCommand : IRequest<bool>
    {

        [DataMember]
        public int OrderNumber { get; private set; }

        public SetPaidOrderStatusCommand(int orderNumber)
        {
            OrderNumber = orderNumber;
        }
    }
}