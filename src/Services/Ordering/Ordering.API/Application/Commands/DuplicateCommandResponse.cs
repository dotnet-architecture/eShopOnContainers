using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Microsoft.eShopOnContainers.Services.Ordering.API.Application.Commands
{
    public abstract class DuplicateCommandResponse<TCommand, TResponse> : IRequestHandler<DuplicateCommandResponse<TCommand, TResponse>.DuplicateCommand<TCommand>, TResponse>
    {
        public class DuplicateCommand<TCommand> : IRequest<TResponse>
        {
            public DuplicateCommand(TCommand command)
            {
                Command = command;
            }

            public TCommand Command { get; }
        }

        /// <summary>
        /// Creates the result value to return if a previous request was found
        /// </summary>
        /// <returns></returns>
        protected abstract Task<TResponse> CreateResponseForDuplicateCommand(TCommand command);

        Task<TResponse> IRequestHandler<DuplicateCommand<TCommand>, TResponse>.Handle(DuplicateCommand<TCommand> request, CancellationToken cancellationToken)
        {
            return CreateResponseForDuplicateCommand(request.Command);
        }
    }
}