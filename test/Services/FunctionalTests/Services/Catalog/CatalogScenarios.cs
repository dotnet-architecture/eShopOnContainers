using Microsoft.eShopOnContainers.Services.Catalog.API.Model;
using Microsoft.eShopOnContainers.Services.Catalog.API.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FunctionalTests.Services.Catalog
{
    public class CatalogScenarios : CatalogScenariosBase
    {
        [Fact]
        public async Task Post_update_a_catalogitem_price_and_catalogitem_is_returned_modified()
        {
            using (var server = CreateServer())
            {
                var client = server.CreateClient();

                // Arrange 
                var itemToModify = GetCatalogItem();
                var newPrice = new Random().Next(1, 200);
                itemToModify.Price = newPrice;

                // Act
                var postRes = await client.PostAsync(Post.UpdateCatalogProduct, 
                    new StringContent(JsonConvert.SerializeObject(itemToModify), 
                    UTF8Encoding.UTF8, "application/json"));
                var response = await client.GetAsync(Get.ProductByName(itemToModify.Name));
                var result = JsonConvert.DeserializeObject<PaginatedItemsViewModel<CatalogItem>>(await response.Content.ReadAsStringAsync());
                var item = result.Data.First();

                // Assert
                Assert.Equal(result.Count, 1);
                Assert.Equal(itemToModify.Id, item.Id);
                Assert.Equal(newPrice, item.Price);
            }

        }

        private CatalogItem GetCatalogItem()
        {
            return new CatalogItem()
            {
                Id = 1,
                Price = 12.5M,
                Name = ".NET Bot Black Sweatshirt"
            };
        }

    }
}
