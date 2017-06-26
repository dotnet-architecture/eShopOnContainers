namespace Microsoft.eShopOnContainers.Services.Catalog.API.Infrastructure
{
    using EntityFrameworkCore;
    using Extensions.Logging;
    using Microsoft.AspNetCore.Builder;
    using Model;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class CatalogContextSeed
    {
        public static async Task SeedAsync(IApplicationBuilder applicationBuilder, ILoggerFactory loggerFactory, int? retry = 0)
        {
            var context = (CatalogContext)applicationBuilder
                .ApplicationServices.GetService(typeof(CatalogContext));

            context.Database.Migrate();

            if (!context.CatalogBrands.Any())
            {
                context.CatalogBrands.AddRange(
                    GetPreconfiguredCatalogBrands());

                await context.SaveChangesAsync();
            }

            if (!context.CatalogTypes.Any())
            {
                context.CatalogTypes.AddRange(
                    GetPreconfiguredCatalogTypes());

                await context.SaveChangesAsync();
            }

            if (!context.CatalogItems.Any())
            {
                context.CatalogItems.AddRange(
                    GetPreconfiguredItems());

                await context.SaveChangesAsync();
            }
        }

        static IEnumerable<CatalogBrand> GetPreconfiguredCatalogBrands()
        {
            return new List<CatalogBrand>()
            {
                new CatalogBrand() { Brand = "Azure"},
                new CatalogBrand() { Brand = ".NET" },
                new CatalogBrand() { Brand = "Visual Studio" },
                new CatalogBrand() { Brand = "SQL Server" }, 
                new CatalogBrand() { Brand = "Other" }
            };
        }

        static IEnumerable<CatalogType> GetPreconfiguredCatalogTypes()
        {
            return new List<CatalogType>()
            {
                new CatalogType() { Type = "Mug"},
                new CatalogType() { Type = "T-Shirt" },
                new CatalogType() { Type = "Sheet" },
                new CatalogType() { Type = "USB Memory Stick" }
            };
        }

        static IEnumerable<CatalogItem> GetPreconfiguredItems()
        {
            return new List<CatalogItem>()
            {
                new CatalogItem() { CatalogTypeId=2,CatalogBrandId=2, PictureFileName = "1.png", Description = ".NET Bot Black Hoodie", Name = ".NET Bot Black Hoodie", Price = 19.5M, PictureUri = "http://externalcatalogbaseurltobereplaced/api/v1/pic/1", AvailableStock = 100},
                new CatalogItem() { CatalogTypeId=1,CatalogBrandId=2, PictureFileName = "2.png", Description = ".NET Black & White Mug", Name = ".NET Black & White Mug", Price= 8.50M, PictureUri = "http://externalcatalogbaseurltobereplaced/api/v1/pic/2", AvailableStock = 100 },
                new CatalogItem() { CatalogTypeId=2,CatalogBrandId=5, PictureFileName = "3.png", Description = "Prism White T-Shirt", Name = "Prism White T-Shirt", Price = 12, PictureUri = "http://externalcatalogbaseurltobereplaced/api/v1/pic/3", AvailableStock = 100 },
                new CatalogItem() { CatalogTypeId=2,CatalogBrandId=2, PictureFileName = "4.png", Description = ".NET Foundation T-shirt", Name = ".NET Foundation T-shirt", Price = 12, PictureUri = "http://externalcatalogbaseurltobereplaced/api/v1/pic/4", AvailableStock = 100 },
                new CatalogItem() { CatalogTypeId=3,CatalogBrandId=5, PictureFileName = "5.png", Description = "Roslyn Red Sheet", Name = "Roslyn Red Sheet", Price = 8.5M, PictureUri = "http://externalcatalogbaseurltobereplaced/api/v1/pic/5", AvailableStock = 100 },
                new CatalogItem() { CatalogTypeId=2,CatalogBrandId=2, PictureFileName = "6.png", Description = ".NET Blue Hoodie", Name = ".NET Blue Hoodie", Price = 12, PictureUri = "http://externalcatalogbaseurltobereplaced/api/v1/pic/6", AvailableStock = 100 },
                new CatalogItem() { CatalogTypeId=2,CatalogBrandId=5, PictureFileName = "7.png", Description = "Roslyn Red T-Shirt", Name = "Roslyn Red T-Shirt", Price = 12, PictureUri = "http://externalcatalogbaseurltobereplaced/api/v1/pic/7", AvailableStock = 100  },
                new CatalogItem() { CatalogTypeId=2,CatalogBrandId=5, PictureFileName = "8.png", Description = "Kudu Purple Hoodie", Name = "Kudu Purple Hoodie", Price = 8.5M, PictureUri = "http://externalcatalogbaseurltobereplaced/api/v1/pic/8", AvailableStock = 100 },
                new CatalogItem() { CatalogTypeId=1,CatalogBrandId=5, PictureFileName = "9.png", Description = "Cup<T> White Mug", Name = "Cup<T> White Mug", Price = 12, PictureUri = "http://externalcatalogbaseurltobereplaced/api/v1/pic/9", AvailableStock = 100 },
                new CatalogItem() { CatalogTypeId=3,CatalogBrandId=2, PictureFileName = "10.png", Description = ".NET Foundation Sheet", Name = ".NET Foundation Sheet", Price = 12, PictureUri = "http://externalcatalogbaseurltobereplaced/api/v1/pic/10", AvailableStock = 100 },
                new CatalogItem() { CatalogTypeId=3,CatalogBrandId=2, PictureFileName = "11.png", Description = "Cup<T> Sheet", Name = "Cup<T> Sheet", Price = 8.5M, PictureUri = "http://externalcatalogbaseurltobereplaced/api/v1/pic/11", AvailableStock = 100 },
                new CatalogItem() { CatalogTypeId=2,CatalogBrandId=5, PictureFileName = "12.png", Description = "Prism White TShirt", Name = "Prism White TShirt", Price = 12, PictureUri = "http://externalcatalogbaseurltobereplaced/api/v1/pic/12", AvailableStock = 100 }
            };           
        }
    }
}
