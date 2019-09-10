using System.Net.Http;
using System.Threading.Tasks;
using System;
using Grpc.Core;
using Serilog;

namespace Microsoft.eShopOnContainers.Web.Shopping.HttpAggregator.Services
{
    public static class GrpcCallerService
    {
        public static async Task<TResponse> CallService<TResponse>(string urlGrpc, Func<HttpClient, Task<TResponse>> func)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2Support", true);

            using var httpClientHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }
            };

            using var httpClient = new HttpClient(httpClientHandler)
            {
                BaseAddress = new Uri(urlGrpc)
            };

            Log.Information("Creating grpc client base address urlGrpc ={@urlGrpc}, BaseAddress={@BaseAddress} ", urlGrpc, httpClient.BaseAddress);

            try
            {
                return await func(httpClient);
            }
            catch (RpcException e)
            {
                Log.Error($"Error calling via grpc: {e.Status} - {e.Message}");
            }

            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", false);
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2Support", false);

            return default;
        }

        public static async Task CallService(string urlGrpc, Func<HttpClient, Task> func)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2Support", true);

            using var httpClientHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }
            };

            using var httpClient = new HttpClient(httpClientHandler)
            {
                BaseAddress = new Uri(urlGrpc)
            };

            Log.Debug("Creating grpc client base address {@httpClient.BaseAddress} ", httpClient.BaseAddress);

            try
            {
                await func(httpClient);
            }
            catch (RpcException e)
            {
                Log.Error($"Error calling via grpc: {e.Status} - {e.Message}");
            }

            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", false);
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2Support", false);
        }
    }
}
