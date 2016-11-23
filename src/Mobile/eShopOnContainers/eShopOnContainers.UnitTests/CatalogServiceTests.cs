using eShopOnContainers.Core.Services.Catalog;
using System.Threading.Tasks;
using Xunit;

namespace eShopOnContainers.UnitTests
{
    public class CatalogServiceTests
    {
        [Fact]
        public async Task GetFakeCatalogTest()
        {
            var catalogMockService = new CatalogMockService();       
            var result  = await catalogMockService.GetCatalogAsync();
            Assert.NotEqual(0, result.Count);
        }
    }
}