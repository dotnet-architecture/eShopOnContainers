﻿using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Ordering.API.Application.Commands
{
    public class SetAwaitingValidationOrderStatusCommand : IRequest<bool>
    {

        [DataMember]
        public int OrderNumber { get; private set; }
        [DataMember]
        public int TenantId { get; private set; }

        public SetAwaitingValidationOrderStatusCommand(int orderNumber, int tenantId)
        {
            OrderNumber = orderNumber;
            TenantId = tenantId;
        }
    }
}