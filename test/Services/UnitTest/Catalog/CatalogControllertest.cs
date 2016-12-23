using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.eShopOnContainers.Services.Catalog.API.Controllers;
using Microsoft.eShopOnContainers.Services.Catalog.API.Infrastructure;
using Microsoft.eShopOnContainers.Services.Catalog.API.Model;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace UnitTest.Catalog
{
    public class CatalogControllerTest
    {
        private readonly Mock<CatalogContext> _mockContext;
        private readonly Mock<IQueryable<CatalogItem>> _mockItems;
        public CatalogControllerTest()
        {
            _mockContext = new Mock<CatalogContext>();
            _mockItems = new Mock<IQueryable<CatalogItem>>();
        }

        [Fact]
        public async Task Items_ReturnsOKObject_WhenItemsFound()
        {
            //CCE: TODO
            Assert.True(true);
        }

    }
}
