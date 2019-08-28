using System.Net.Http;
using System.Threading.Tasks;
using System;
using Grpc.Core;
using Serilog;

namespace Microsoft.eShopOnContainers.Web.Shopping.HttpAggregator.Services
{
    public static class GrpcCallerService
    {
        public static Task<TResponse> CallService<TResponse>(string urlGrpc, Func<HttpClient, Task<TResponse>> func)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2Support", true);

            using (var httpClientHandler = new HttpClientHandler())
            {
                httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
                using (var httpClient = new HttpClient(httpClientHandler))
                {
                    httpClient.BaseAddress = new Uri(urlGrpc);
                    Log.Debug("Creating grpc client base address {@httpClient.BaseAddress} ", httpClient.BaseAddress);

                    try
                    {
                        return func(httpClient);
                    }
                    catch (RpcException e)
                    {
                        Log.Error($"Error calling via grpc: {e.Status} - {e.Message}");
                    }
                }
            }

            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", false);
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2Support", false);

            return default;
        }

        public static Task CallService(string urlGrpc, Func<HttpClient, Task> func)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2Support", true);

            using (var httpClientHandler = new HttpClientHandler())
            {
                httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
                using (var httpClient = new HttpClient(httpClientHandler))
                {
                    httpClient.BaseAddress = new Uri(urlGrpc);
                    Log.Debug("Creating grpc client base address {@httpClient.BaseAddress} ", httpClient.BaseAddress);

                    try
                    {
                        return func(httpClient);
                    }
                    catch (RpcException e)
                    {
                        Log.Error($"Error calling via grpc: {e.Status} - {e.Message}");
                    }
                }
            }

            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", false);
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2Support", false);

            return default;
        }
    }
}
