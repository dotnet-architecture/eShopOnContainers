using System;
using System.Threading.Tasks;
using eShopOnContainers.Core.Services.RequestProvider;
using eShopOnContainers.Core.Models.Basket;

namespace eShopOnContainers.Core.Services.Basket
{
    public class BasketService : IBasketService
    {
        private readonly IRequestProvider _requestProvider;

        public BasketService(IRequestProvider requestProvider)
        {
            _requestProvider = requestProvider;
        }

        public async Task<CustomerBasket> GetBasketAsync(string guidUser)
        {
            try
            {
                UriBuilder builder = new UriBuilder(GlobalSetting.CatalogEndpoint);

                builder.Path = guidUser;

                string uri = builder.ToString();

                CustomerBasket basket =
                     await _requestProvider.GetAsync<CustomerBasket>(uri);

                return basket;
            }
            catch
            {
                return new CustomerBasket
                {
                    BuyerId = guidUser,
                    Items = new System.Collections.Generic.List<BasketItem>()
                };
            }
        }

        public async Task<CustomerBasket> UpdateBasketAsync(CustomerBasket customerBasket)
        {
            UriBuilder builder = new UriBuilder(GlobalSetting.CatalogEndpoint);

            string uri = builder.ToString();

            var result = await _requestProvider.PostAsync(uri, customerBasket);

            return result;
        }
    }
}