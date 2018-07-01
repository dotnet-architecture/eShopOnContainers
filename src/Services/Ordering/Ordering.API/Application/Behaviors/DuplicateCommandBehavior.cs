using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.eShopOnContainers.Services.Ordering.API.Application.Commands;
using Microsoft.eShopOnContainers.Services.Ordering.Infrastructure.Idempotency;
using Microsoft.Extensions.Logging;

namespace Ordering.API.Infrastructure.Behaviors
{
    public class DuplicateCommandBehavior<TCommand, TResponse> : IPipelineBehavior<TCommand, TResponse>
        where TCommand : ICommand
    {
        private readonly IMediator _mediator;
        private readonly IRequestManager _requestManager;

        public DuplicateCommandBehavior(IMediator mediator, IRequestManager requestManager)
        {
            _mediator = mediator;
            _requestManager = requestManager;
        }

        async Task<TResponse> IPipelineBehavior<TCommand, TResponse>.Handle(TCommand command, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var alreadyExists = await _requestManager.ExistAsync(command.CommandId);
            if (alreadyExists)
            {
                var duplicateCommand = new DuplicateCommandResponse<TCommand, TResponse>.DuplicateCommand<TCommand>(command);
                return await _mediator.Send(duplicateCommand, cancellationToken);
            }

            await _requestManager.CreateRequestForCommandAsync<TCommand>(command.CommandId);
            
            var response = await next();
            return response;
        }
    }
}