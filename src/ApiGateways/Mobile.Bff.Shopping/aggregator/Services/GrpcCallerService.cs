using System.Net.Http;
using System.Threading.Tasks;
using System;
using Grpc.Core;
using Serilog;
using Polly;
using Grpc.Net.Client;

namespace Microsoft.eShopOnContainers.Mobile.Shopping.HttpAggregator.Services
{
    public static class GrpcCallerService
    {
        public static async Task<TResponse> CallService<TResponse>(string urlGrpc, Func<GrpcChannel, Task<TResponse>> func)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2Support", true);

            var channel = GrpcChannel.ForAddress(urlGrpc);

            /*
            using var httpClientHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }
            };

            */


            Log.Information("Creating grpc client base address urlGrpc ={@urlGrpc}, BaseAddress={@BaseAddress} ", urlGrpc, channel.Target);

            try
            {
                return await Policy
                                .Handle<Exception>(ex => true)
                                .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (e, t) => Log.Warning("Retrying the call to urlGrpc ={@urlGrpc}, BaseAddress={@BaseAddress}, errorMessage={@message}", urlGrpc, channel.Target, e.Message))
                                .ExecuteAsync(() => func(channel))
                                ;
            }
            catch (RpcException e)
            {
                Log.Error($"Error calling via grpc: {e.Status} - {e.Message}");
                return default;
            }
            finally
            {
                AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", false);
                AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2Support", false);
            }


        }

        public static async Task CallService(string urlGrpc, Func<GrpcChannel, Task> func)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2Support", true);

            /*
            using var httpClientHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }
            };
            */

            var channel = GrpcChannel.ForAddress(urlGrpc);

            Log.Debug("Creating grpc client base address {@httpClient.BaseAddress} ", channel.Target);

            try
            {
                await Policy
                        .Handle<Exception>(ex => true)
                        .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (e, t) => Log.Warning("Retrying the call to urlGrpc ={@urlGrpc}, BaseAddress={@BaseAddress}, errorMessage={@message}", urlGrpc, channel.Target, e.Message))
                        .ExecuteAsync(() => func(channel))
                        ;
            }
            catch (RpcException e)
            {
                Log.Error($"Error calling via grpc: {e.Status} - {e.Message}");
            }
            finally
            {
                AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", false);
                AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2Support", false);
            }
        }
    }
}
