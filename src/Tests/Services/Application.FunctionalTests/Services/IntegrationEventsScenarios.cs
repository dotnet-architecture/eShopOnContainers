using FunctionalTests.Services.Basket;
using FunctionalTests.Services.Catalog;
using Microsoft.eShopOnContainers.Services.Basket.API.Model;
using Microsoft.eShopOnContainers.Services.Catalog.API.Model;
using Microsoft.eShopOnContainers.Services.Catalog.API.ViewModel;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using System.Net.Http;
using System.Threading;

namespace FunctionalTests.Services
{
    public class IntegrationEventsScenarios
    {
        [Fact]
        public async Task Post_update_product_price_and_catalog_and_basket_list_modified()
        {
            decimal priceModification = 0.15M;
            string userId = "JohnId";

            using (var catalogServer = new CatalogScenariosBase().CreateServer())
            using (var basketServer = new BasketScenariosBase().CreateServer())
            {
                var catalogClient = catalogServer.CreateClient();
                var basketClient = basketServer.CreateClient();

                // GIVEN a product catalog list                           
                var originalCatalogProducts = await GetCatalogAsync(catalogClient);

                // AND a user basket filled with products   
                var basket = ComposeBasket(userId, originalCatalogProducts.Data.Take(3));
                var res = await basketClient.PostAsync(
                    BasketScenariosBase.Post.CreateBasket,
                    new StringContent(JsonConvert.SerializeObject(basket), UTF8Encoding.UTF8, "application/json")
                    );

                // WHEN the price of one product is modified in the catalog
                var itemToModify = basket.Items[2];
                var oldPrice = itemToModify.UnitPrice;
                var newPrice = oldPrice + priceModification;
                var pRes = await catalogClient.PutAsync(CatalogScenariosBase.Put.UpdateCatalogProduct, new StringContent(ChangePrice(itemToModify, newPrice, originalCatalogProducts), UTF8Encoding.UTF8, "application/json"));
                                
                var modifiedCatalogProducts = await GetCatalogAsync(catalogClient);               

                var itemUpdated = await GetUpdatedBasketItem(newPrice, itemToModify.ProductId, userId, basketClient);

                if (itemUpdated == null)
                {
                    Assert.False(true, $"The basket service has not been updated.");
                }
                else
                {
                    //THEN the product price changes in the catalog 
                    Assert.Equal(newPrice, modifiedCatalogProducts.Data.Single(it => it.Id == itemToModify.ProductId).Price);

                    // AND the products in the basket reflects the changed priced and the original price
                    Assert.Equal(newPrice, itemUpdated.UnitPrice);
                    Assert.Equal(oldPrice, itemUpdated.OldUnitPrice);
                }
            }
        }

        private async Task<BasketItem> GetUpdatedBasketItem(decimal newPrice, int productId, string userId, HttpClient basketClient)
        {
            bool continueLoop = true;
            var counter = 0;
            BasketItem itemUpdated = null;

            while (continueLoop && counter < 20)
            {                
                //get the basket and verify that the price of the modified product is updated
                var basketGetResponse = await basketClient.GetAsync(BasketScenariosBase.Get.GetBasketByCustomer(userId));
                var basketUpdated = JsonConvert.DeserializeObject<CustomerBasket>(await basketGetResponse.Content.ReadAsStringAsync());

                itemUpdated = basketUpdated.Items.Single(pr => pr.ProductId == productId);

                if (itemUpdated.UnitPrice == newPrice)
                {
                    continueLoop = false;
                }
                else
                {
                    counter++;
                    await Task.Delay(100);
                }
            }

            return itemUpdated;
        }

        private async  Task<PaginatedItemsViewModel<CatalogItem>> GetCatalogAsync(HttpClient catalogClient)
        {
            var response = await catalogClient.GetAsync(CatalogScenariosBase.Get.Items);
            var items = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<PaginatedItemsViewModel<CatalogItem>>(items);
        }

        private string ChangePrice(BasketItem itemToModify, decimal newPrice, PaginatedItemsViewModel<CatalogItem> catalogProducts)
        {
            var catalogProduct = catalogProducts.Data.Single(pr => pr.Id == itemToModify.ProductId);
            catalogProduct.Price = newPrice;
            return JsonConvert.SerializeObject(catalogProduct);
        }

        private CustomerBasket ComposeBasket(string customerId, IEnumerable<CatalogItem> items)
        {
            var basket = new CustomerBasket(customerId);
            foreach (var item in items)
            {
                basket.Items.Add(new BasketItem()
                {
                    Id = Guid.NewGuid().ToString(),
                    UnitPrice = item.Price,
                    PictureUrl = item.PictureUri,
                    ProductId = item.Id,
                    OldUnitPrice = 0,
                    ProductName = item.Name,
                    Quantity = 1
                });
            }
            return basket;
        }
    }
}