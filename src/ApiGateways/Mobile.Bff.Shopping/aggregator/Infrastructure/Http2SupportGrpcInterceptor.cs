using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;
using System;

namespace Microsoft.eShopOnContainers.Mobile.Shopping.HttpAggregator.Infrastructure
{
    public class Http2SupportGrpcInterceptor : Interceptor
    {
        private readonly ILogger<Http2SupportGrpcInterceptor> _logger;

        public Http2SupportGrpcInterceptor(ILogger<Http2SupportGrpcInterceptor> logger)
        {
            _logger = logger;
        }

        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(
            TRequest request,
            ClientInterceptorContext<TRequest, TResponse> context,
            AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2Support", true);

            _logger.LogInformation("Calling via grpc client base address serviceName ={@ServiceName}, BaseAddress={@Host} ", context.Method.ServiceName, context.Host);

            try
            {
                return continuation(request, context);
            }
            catch (RpcException e)
            {
                _logger.LogError("Error calling via grpc: {Status} - {Message}", e.Status, e.Message);
                return default;
            }
            finally
            {
                AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", false);
                AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2Support", false);
            }
        }
    }
}
