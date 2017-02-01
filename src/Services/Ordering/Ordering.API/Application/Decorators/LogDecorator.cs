namespace Microsoft.eShopOnContainers.Services.Ordering.API.Application.Decorators
{
    using Extensions.Logging;
    using MediatR;
    using System.Threading.Tasks;

    public class LogDecorator<TRequest, TResponse>
        : IAsyncRequestHandler<TRequest, TResponse>
         where TRequest : IAsyncRequest<TResponse>
    {
        private readonly IAsyncRequestHandler<TRequest, TResponse> _inner;
        private readonly ILogger<LogDecorator<TRequest, TResponse>> _logger;

       
        public LogDecorator(
            IAsyncRequestHandler<TRequest, TResponse> inner,
            ILogger<LogDecorator<TRequest, TResponse>> logger)
        {
            _inner = inner;
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest message)
        {
            _logger.LogInformation($"Executing command {_inner.GetType().FullName}");

            var response = await _inner.Handle(message);

            _logger.LogInformation($"Succedded executed command {_inner.GetType().FullName}");

            return response;
        }
    }
}
