namespace Microsoft.eShopOnContainers.Services.Catalog.API.Infrastructure
{
    using EntityFrameworkCore;
    using Extensions.Logging;
    using Microsoft.AspNetCore.Builder;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class CatalogContextSeed
    {
        public static async Task SeedAsync(IApplicationBuilder applicationBuilder, ILoggerFactory loggerFactory, int? retry = 0)
        {
            int retryForAvaiability = retry.Value;
            try
            {
                var context = (CatalogContext)applicationBuilder
                    .ApplicationServices.GetService(typeof(CatalogContext));

                //using (context)
                //{
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
                //}
            }
            catch (Exception ex)
            {
                if (retryForAvaiability < 10)
                {
                    retryForAvaiability++;
                    var log = loggerFactory.CreateLogger("catalog seed");
                    log.LogError(ex.Message);
                    await SeedAsync(applicationBuilder, loggerFactory, retryForAvaiability);
                }
            }
        }

        static IEnumerable<CatalogBrand> GetPreconfiguredCatalogBrands()
        {
            return new List<CatalogBrand>()
            {
                new CatalogBrand() { Brand = "Azure"},
                new CatalogBrand() { Brand = ".NET" },
                new CatalogBrand() { Brand = "Visual Studio" },
                new CatalogBrand() { Brand = "SQL Server" }
            };
        }

        static IEnumerable<CatalogType> GetPreconfiguredCatalogTypes()
        {
            return new List<CatalogType>()
            {
                new CatalogType() { Type = "Mug"},
                new CatalogType() { Type = "T-Shirt" },
                new CatalogType() { Type = "Backpack" },
                new CatalogType() { Type = "USB Memory Stick" }
            };
        }

        static IEnumerable<CatalogItem> GetPreconfiguredItems()
        {
            return new List<CatalogItem>()
            {
                new CatalogItem() { CatalogTypeId=2,CatalogBrandId=2, Description = "Roslyn Red T-Shirt", Name = "Roslyn Red T-Shirt", Price = 12, PictureUri = "https://fakeimg.pl/370x240/EEEEEE/000/?text=RoslynRedT-Shirt"  },
                new CatalogItem() { CatalogTypeId=1,CatalogBrandId=2, Description = "Cupt Black & White Mug", Name = "Cupt Black & White Mug", Price= 17, PictureUri = "https://fakeimg.pl/370x240/EEEEEE/000/?text=CuptBlack&WhiteMug" },
                new CatalogItem() { CatalogTypeId=2,CatalogBrandId=2, Description = "Prism White T-Shirt", Name = "Prism White T-Shirt", Price = 12, PictureUri = "http://fakeimg.pl/370x240/EEEEEE/000/?text=.PrismWhiteT-Shirt" },
                new CatalogItem() { CatalogTypeId=2,CatalogBrandId=1, Description = ".NET Bot Black Sweatshirt", Name = ".NET Bot Black Sweatshirt", Price = 19.5M, PictureUri = "http://fakeimg.pl/370x240/EEEEEE/000/?text=.NETBotBlack" },
            };
        }
    }
}
