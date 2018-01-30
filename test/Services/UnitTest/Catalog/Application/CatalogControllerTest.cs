using Microsoft.AspNetCore.Mvc;
using Microsoft.eShopOnContainers.Services.Common.API;
using Microsoft.eShopOnContainers.WebMVC.Controllers;
using Microsoft.eShopOnContainers.WebMVC.Services;
using Microsoft.eShopOnContainers.WebMVC.ViewModels;
using Microsoft.eShopOnContainers.WebMVC.ViewModels.CatalogViewModels;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using CatalogModel = Microsoft.eShopOnContainers.Services.Common.API.PaginatedItemsViewModel<Microsoft.eShopOnContainers.WebMVC.ViewModels.CatalogItem>;

namespace UnitTest.Catalog.Application
{
    public class CatalogControllerTest
    {
        private readonly Mock<ICatalogService> _catalogServiceMock;

        public CatalogControllerTest()
        {
            _catalogServiceMock = new Mock<ICatalogService>();
        }

        [Fact]
        public async Task Get_catalog_items_success()
        {
            //Arrange
            var fakeBrandFilterApplied = 1;
            var fakeTypesFilterApplied = 2;
            var fakePage = 2;
            var fakeCatalog = GetFakeCatalog();

            var expectedNumberOfPages = 5;
            var expectedTotalPages = 50;
            var expectedCurrentPage = 2;

            _catalogServiceMock.Setup(x => x.GetCatalogItems
            (
                It.Is<int>(y => y == fakePage),
                It.IsAny<int>(),
                It.Is<int?>(y => y == fakeBrandFilterApplied),
                It.Is<int?>(y => y == fakeTypesFilterApplied)
             ))
             .Returns(Task.FromResult(fakeCatalog));

            //Act
            var orderController = new CatalogController(_catalogServiceMock.Object);
            var actionResult = await orderController.Index(fakeBrandFilterApplied, fakeTypesFilterApplied, fakePage, null);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(actionResult);
            var model = Assert.IsAssignableFrom<IndexViewModel>(viewResult.ViewData.Model);
            Assert.Equal(model.PaginationInfo.TotalPages, expectedNumberOfPages);
            Assert.Equal(model.PaginationInfo.TotalItems, expectedTotalPages);
            Assert.Equal(model.PaginationInfo.ActualPage, expectedCurrentPage);
            Assert.Empty(model.PaginationInfo.Next);
            Assert.Empty(model.PaginationInfo.Previous);
        }   
        
        private PaginatedItemsViewModel<CatalogItem> GetFakeCatalog()
        {
			return new PaginatedItemsViewModel<CatalogItem>(2, 10, 50, new List<CatalogItem>
			{
					new CatalogItem()
					{
						Id = "1",
						Name = "fakeItemA",
						CatalogTypeId = 1
					},
					new CatalogItem()
					{
						Id = "2",
						Name = "fakeItemB",
						CatalogTypeId = 1
					},
					new CatalogItem()
					{
						Id = "3",
						Name = "fakeItemC",
						CatalogTypeId = 1
					}
			});
        }
    }
}
