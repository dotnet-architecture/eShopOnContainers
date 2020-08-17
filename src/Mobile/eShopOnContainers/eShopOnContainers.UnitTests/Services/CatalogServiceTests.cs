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
            var catalog = await catalogMockService.GetCatalogAsync();

            Assert.NotEmpty(catalog);
        }

        [Fact]
        public async Task GetFakeCatalogBrandTest()
        {
            var catalogMockService = new CatalogMockService();
            var catalogBrand = await catalogMockService.GetCatalogBrandAsync();

            Assert.NotEmpty(catalogBrand);
        }

        [Fact]
        public async Task GetFakeCatalogTypeTest()
        {
            var catalogMockService = new CatalogMockService();
            var catalogType = await catalogMockService.GetCatalogTypeAsync();

            Assert.NotEmpty(catalogType);
        }
    }
}