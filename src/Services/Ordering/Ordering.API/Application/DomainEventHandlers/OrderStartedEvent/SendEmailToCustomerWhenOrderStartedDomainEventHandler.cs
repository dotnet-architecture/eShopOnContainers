using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MediatR;
using Ordering.Domain.Events;

namespace Ordering.API.Application.DomainEventHandlers.OrderStartedEvent
{
    public class SendEmailToCustomerWhenOrderStartedDomainEventHandler
                   //: IAsyncNotificationHandler<OrderStartedDomainEvent>
    { 
        public SendEmailToCustomerWhenOrderStartedDomainEventHandler()
        {
        
        }

        //public async Task Handle(OrderStartedDomainEvent orderNotification)
        //{
        //    //TBD - Send email logic
        //}
    }
}
