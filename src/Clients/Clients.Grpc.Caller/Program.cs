using System;
using System.Net.Http;
using System.Threading.Tasks;
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
                    var reply = await client.GetBasketByIdAsync(
                                      new BasketRequest { Id = "11" });
                    Console.WriteLine("Greeting: " + reply.Buyerid);
                    Console.WriteLine("Press any key to exit...");
                    Console.ReadKey();
                }
            }
        }
    }
}
