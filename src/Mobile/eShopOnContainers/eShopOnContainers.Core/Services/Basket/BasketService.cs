using System;
using System.Threading.Tasks;
using eShopOnContainers.Core.Services.RequestProvider;
using eShopOnContainers.Core.Models.Basket;
using eShopOnContainers.Core.Services.FixUri;

namespace eShopOnContainers.Core.Services.Basket
{
    public class BasketService : IBasketService
    {
        private readonly IRequestProvider _requestProvider;
        private readonly IFixUriService _fixUriService;

        private const string ApiUrlBase = "mobileshoppingapigw/api/v1/b/basket";

        public BasketService(IRequestProvider requestProvider, IFixUriService fixUriService)
        {
            _requestProvider = requestProvider;
            _fixUriService = fixUriService;
        }

        public async Task<CustomerBasket> GetBasketAsync(string guidUser, string token)
        {
            var builder = new UriBuilder(GlobalSetting.Instance.BaseEndpoint)
            {
                Path = $"{ApiUrlBase}/{guidUser}"
            };

            var uri = builder.ToString();

            CustomerBasket basket;

            try
            {
                basket = await _requestProvider.GetAsync<CustomerBasket>(uri, token);
            }
            catch (HttpRequestExceptionEx exception) when (exception.HttpCode == System.Net.HttpStatusCode.NotFound)
            {
                basket = null;
            }

            _fixUriService.FixBasketItemPictureUri(basket?.Items);
            return basket;
        }

        public async Task<CustomerBasket> UpdateBasketAsync(CustomerBasket customerBasket, string token)
        {
            var builder = new UriBuilder(GlobalSetting.Instance.BaseEndpoint)
            {
                Path = ApiUrlBase
            };

            var uri = builder.ToString();
            var result = await _requestProvider.PostAsync(uri, customerBasket, token);
            return result;
        }

        public async Task CheckoutAsync(BasketCheckout basketCheckout, string token)
        {
            var builder = new UriBuilder(GlobalSetting.Instance.BaseEndpoint)
            {
                Path = $"{ApiUrlBase}/checkout"
            };

            var uri = builder.ToString();
            await _requestProvider.PostAsync(uri, basketCheckout, token);
        }

        public async Task ClearBasketAsync(string guidUser, string token)
        {
            var builder = new UriBuilder(GlobalSetting.Instance.BaseEndpoint)
            {
                Path = $"{ApiUrlBase}/{guidUser}"
            };

            var uri = builder.ToString();
            await _requestProvider.DeleteAsync(uri, token);
        }
    }
}