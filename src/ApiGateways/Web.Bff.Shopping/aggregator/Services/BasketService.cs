using Microsoft.eShopOnContainers.Web.Shopping.HttpAggregator.Config;
using Microsoft.eShopOnContainers.Web.Shopping.HttpAggregator.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.eShopOnContainers.Web.Shopping.HttpAggregator.Services
{
    public class BasketService : IBasketService
    {
        private readonly HttpClient _apiClient;
        private readonly ILogger<BasketService> _logger;
        private readonly UrlsConfig _urls;

        public BasketService(HttpClient httpClient,ILogger<BasketService> logger, IOptions<UrlsConfig> config)
        {
            _apiClient = httpClient;
            _logger = logger;
            _urls = config.Value;
        }

        public async Task<BasketData> GetByIdAsync(string id)
        {

            _logger.LogInformation("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ GetById @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");

            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2Support", true);
  using (var httpClientHandler = new HttpClientHandler())
            {
                httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
                using (var httpClient = new HttpClient(httpClientHandler))
                {

            _logger.LogInformation("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ Http2UnencryptedSupport disable @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");

            httpClient.BaseAddress = new Uri("http://localhost:5580");

            _logger.LogInformation("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ {httpClient.BaseAddress} @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@", httpClient.BaseAddress);

            var client = GrpcClient.Create<BasketClient>(httpClient);

            _logger.LogInformation("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ client create @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");

            try{

            var response = await client.GetBasketByIdAsync(new BasketRequest { Id = id });
            _logger.LogInformation("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ call grpc server @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");

            _logger.LogInformation("############## DATA: {@a}", response.Buyerid);
            _logger.LogInformation("############## DATA:response {@response}", response);
                    }
                    catch (RpcException e)
                    {
                        Console.WriteLine($"Error calling via grpc: {e.Status} - {e.Message}");
                        _logger.logError($"Error calling via grpc: {e.Status} - {e.Message}");

                    }  


            //if (streaming.IsCompleted)
            //{
            //    _logger.LogInformation("############## DATA: {@a}", streaming.GetResult());
            //}
            //var streaming = client.GetBasketById(new BasketRequest { Id = id });


            //var status = streaming.GetStatus();

            //if (status.StatusCode == Grpc.Core.StatusCode.OK)
            //{
            //    return null;
            //}

            return response;

                }
                }

            // var data = await _apiClient.GetStringAsync(_urls.Basket +  UrlsConfig.BasketOperations.GetItemById(id));
            // var basket = !string.IsNullOrEmpty(data) ? JsonConvert.DeserializeObject<BasketData>(data) : null;

            // return basket;
        }

        public async Task UpdateAsync(BasketData currentBasket)
        {
            var basketContent = new StringContent(JsonConvert.SerializeObject(currentBasket), System.Text.Encoding.UTF8, "application/json");

            await _apiClient.PostAsync(_urls.Basket + UrlsConfig.BasketOperations.UpdateBasket(), basketContent);
        }
    }
}
