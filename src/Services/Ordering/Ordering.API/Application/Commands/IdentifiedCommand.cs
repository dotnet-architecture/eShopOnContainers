using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.eShopOnContainers.Services.Ordering.API.Application.Commands
{
    public class IdentifiedCommand<T, R> : IAsyncRequest<R>
        where T : IAsyncRequest<R>
    {
        public T Command { get; }
        public Guid Id { get; }
        public IdentifiedCommand(T command, Guid id)
        {
            Command = command;
            Id = id;
        }
    }
}
