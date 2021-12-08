﻿namespace Microsoft.eShopOnContainers.Services.Ordering.API.Application.Commands;

public class SetStockConfirmedOrderStatusCommand : IRequest<bool>
{

    [DataMember]
    public int OrderNumber { get; private set; }

    public SetStockConfirmedOrderStatusCommand(int orderNumber)
    {
        OrderNumber = orderNumber;
    }
}
