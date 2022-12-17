using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Domain.Events;
public class OrderStatusChangedToAwaitingCouponValidationDomainEvent : INotification
{
    public int OrderId { get; }

    public string Code { get; set; }

    public OrderStatusChangedToAwaitingCouponValidationDomainEvent(int orderId, string code)
    {
        OrderId = orderId;
        Code = code;
    }
}
