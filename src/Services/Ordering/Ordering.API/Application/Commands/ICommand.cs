using System;

namespace Microsoft.eShopOnContainers.Services.Ordering.API.Application.Commands
{
    public interface ICommand
    {
        Guid CommandId { get; }
    }
}