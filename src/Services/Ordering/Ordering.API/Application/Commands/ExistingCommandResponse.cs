using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Microsoft.eShopOnContainers.Services.Ordering.API.Application.Commands
{
    public abstract class ExistingCommandResponse<TCommand, TResponse> : IRequestHandler<ExistingCommandResponse<TCommand, TResponse>.ExistingCommand, TResponse>
    {
        public class ExistingCommand : IRequest<TResponse>
        {
            public ExistingCommand(TCommand command)
            {
                Command = command;
            }

            public TCommand Command { get; }
        }

        /// <summary>
        /// Creates the result value to return if a previous request was found
        /// </summary>
        /// <returns></returns>
        protected abstract Task<TResponse> CreateResponseForExistingCommand(TCommand command);

        Task<TResponse> IRequestHandler<ExistingCommand, TResponse>.Handle(ExistingCommand request, CancellationToken cancellationToken)
        {
            return CreateResponseForExistingCommand(request.Command);
        }
    }
}