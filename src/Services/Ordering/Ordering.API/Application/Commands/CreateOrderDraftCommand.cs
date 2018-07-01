using MediatR;
using Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel.OrderAggregate;
using Ordering.API.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using static Microsoft.eShopOnContainers.Services.Ordering.API.Application.Commands.CreateOrderCommand;

namespace Microsoft.eShopOnContainers.Services.Ordering.API.Application.Commands
{
    public class CreateOrderDraftCommand : ICommand, IRequest<OrderDraftDTO>
    {
        public Guid CommandId { get; }

        public string BuyerId { get; private set; }

        public IEnumerable<BasketItem> Items { get; private set; }

        public CreateOrderDraftCommand(Guid id, string buyerId, IEnumerable<BasketItem> items)
        {
            CommandId = id;
            BuyerId = buyerId;
            Items = items;
        }
    }

}
