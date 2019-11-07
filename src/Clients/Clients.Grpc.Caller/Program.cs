using System;
using System.Net.Http;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using GrpcBasket;
namespace Clients.Grpc.Caller
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Starting...");
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2Support", true);

            using (var httpClientHandler = new HttpClientHandler())
            {
                httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
                using (var httpClient = new HttpClient(httpClientHandler))
                {
                    // Make your request...
                    //  var httpClient = new HttpClient();
                    // The port number(5001) must match the port of the gRPC server.
                    //httpClient.BaseAddress = new Uri("http://localhost:5103");
                    httpClient.BaseAddress = new Uri("http://localhost:5580");
                    //httpClient.DefaultRequestVersion = Version.Parse("2.0");
                    var client = GrpcClient.Create<Basket.BasketClient>(httpClient);

                    try
                    {
                        var reply = await client.GetBasketByIdAsync(
                                          new BasketRequest { Id = "4f71a02f-4738-43a9-8c81-7652877e7102" });
                        Console.WriteLine("Greeting: " + reply.Buyerid);
                        Console.WriteLine("Greeting: " + reply.Items);

                    }
                    //catch(Grpc)
                    catch (RpcException e)
                    {
                        Console.WriteLine($"Error calling via grpc: {e.Status} - {e.Message}");
                    }

                    Console.WriteLine("Press any key to exit...");
                    Console.ReadKey();
                }
            }
        }
    }
}
