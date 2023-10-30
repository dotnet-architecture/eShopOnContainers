using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Domain.Events
{
    public class OrderCompletedDomainEvent : INotification
    {
        public Order Order { get; }

        public OrderCompletedDomainEvent(Order order)
        {
            Order = order;
        }
    }
}
