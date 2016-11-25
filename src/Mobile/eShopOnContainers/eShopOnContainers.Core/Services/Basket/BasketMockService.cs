using eShopOnContainers.Core.Models.Basket;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace eShopOnContainers.Core.Services.Basket
{
    public class BasketMockService : IBasketService
    {
        private CustomerBasket MockCustomBasket = new CustomerBasket
        {
            BuyerId = "9245fe4a-d402-451c-b9ed-9c1a04247482",
            Items = new List<BasketItem>
                {
                    new BasketItem { Id = "1", PictureUrl = Device.OS != TargetPlatform.Windows ? "fake_product_01.png" : "Assets/fake_product_01.png", ProductId = "1", ProductName = ".NET Bot Blue Sweatshirt (M)", Quantity = 1, UnitPrice = 19.50M },
                    new BasketItem { Id = "2", PictureUrl = Device.OS != TargetPlatform.Windows ? "fake_product_04.png" : "Assets/fake_product_04.png", ProductId = "4", ProductName = ".NET Black Cupt", Quantity = 1, UnitPrice = 17.00M }
                }
        };

        public async Task<CustomerBasket> GetBasketAsync(string guidUser)
        {
            await Task.Delay(500);

            if(string.IsNullOrEmpty(guidUser))
            {
                return new CustomerBasket();
            }

            return MockCustomBasket;
        }

        public async Task<CustomerBasket> UpdateBasketAsync(CustomerBasket customerBasket)
        {
            await Task.Delay(500);

            MockCustomBasket = customerBasket;

            return MockCustomBasket;
        }

        public async Task ClearBasketAsync(string guidUser)
        {
            await Task.Delay(500);

            if (!string.IsNullOrEmpty(guidUser))
            {
                MockCustomBasket.Items.Clear();
            }
        }
    }
}