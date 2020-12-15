using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;

namespace Microsoft.eShopOnContainers.Mobile.Shopping.HttpAggregator.Infrastructure
{
    public class GrpcExceptionInterceptor : Interceptor
    {
        private readonly ILogger<GrpcExceptionInterceptor> _logger;

        public GrpcExceptionInterceptor(ILogger<GrpcExceptionInterceptor> logger)
        {
            _logger = logger;
        }

        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(
            TRequest request,
            ClientInterceptorContext<TRequest, TResponse> context,
            AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            _logger.LogInformation("Calling via grpc client base address serviceName={@ServiceName}, BaseAddress={@Host}", context.Method.ServiceName, context.Host);

            try
            {
                return continuation(request, context);
            }
            catch (RpcException e)
            {
                _logger.LogError("Error calling via grpc: {Status} - {Message}", e.Status, e.Message);
                return default;
            }
        }
    }
}
