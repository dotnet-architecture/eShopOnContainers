using System;
using MediatR;

namespace Microsoft.eShopOnContainers.Services.Ordering.API.Application.Commands
{
    public interface ICommand
    {
        Guid CommandId { get; }
    }

    public interface ICommand<out TResponse> : ICommand, IRequest<TResponse>
    {
    }
}