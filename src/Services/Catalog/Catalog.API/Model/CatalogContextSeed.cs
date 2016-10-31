namespace Microsoft.eShopOnContainers.Services.Catalog.API.Model
{
    using Microsoft.AspNetCore.Builder;
    using Model;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class CatalogContextSeed
    {
        public static async Task SeedAsync(IApplicationBuilder applicationBuilder)
        {
            var context = (CatalogContext)applicationBuilder
                .ApplicationServices.GetService(typeof(CatalogContext));

            using (context)
            {
                context.Database.EnsureCreated();

                if (!context.CatalogItems.Any())
                {
                    context.CatalogItems.AddRange(
                        GetPreconfiguredItems());

                    await context.SaveChangesAsync();
                }
            }
        }

        static IEnumerable<CatalogItem> GetPreconfiguredItems()
        {
            return new List<CatalogItem>()
            {
                new CatalogItem() {  Description = "Roslyn Red T-Shirt", Name = "Roslyn Red T-Shirt", Price = 12, PictureUri = "https://fakeimg.pl/370x240/EEEEEE/000/?text=RoslynRedT-Shirt"  },
                new CatalogItem() {  Description = "Cupt Black & White Mug", Name = "Cupt Black & White Mug", Price= 17, PictureUri = "https://fakeimg.pl/370x240/EEEEEE/000/?text=CuptBlack&WhiteMug" },
                new CatalogItem() {  Description = "Prism White T-Shirt", Name = "Prism White T-Shirt", Price = 12, PictureUri = "http://fakeimg.pl/370x240/EEEEEE/000/?text=.PrismWhiteT-Shirt" },
                new CatalogItem() {  Description = ".NET Bot Black Sweatshirt", Name = ".NET Bot Black Sweatshirt", Price = 19.5M, PictureUri = "http://fakeimg.pl/370x240/EEEEEE/000/?text=.NETBotBlack" },
                new CatalogItem() {  Description = "Roslyn Red T-Shirt", Name = "Roslyn Red T-Shirt", Price = 12, PictureUri = "http://fakeimg.pl/370x240/EEEEEE/000/?text=RoslynRedT-Shirt"  },
                new CatalogItem() {  Description = "Cupt Black & White Mug", Name = "Cupt Black & White Mug", Price= 17, PictureUri = "https://fakeimg.pl/370x240/EEEEEE/000/?text=CuptBlack&WhiteMug" },
                new CatalogItem() {  Description = "Prism White T-Shirt", Name = "Prism White T-Shirt", Price = 12, PictureUri = "http://fakeimg.pl/370x240/EEEEEE/000/?text=.PrismWhiteT-Shirt" },
                new CatalogItem() {  Description = ".NET Bot Black Sweatshirt", Name = ".NET Bot Black Sweatshirt", Price = 19.5M, PictureUri = "http://fakeimg.pl/370x240/EEEEEE/000/?text=.NETBotBlack" },
                new CatalogItem() {  Description = "Roslyn Red T-Shirt", Name = "Roslyn Red T-Shirt", Price = 12, PictureUri = "http://fakeimg.pl/370x240/EEEEEE/000/?text=RoslynRedT-Shirt"  },
                new CatalogItem() {  Description = "Cupt Black & White Mug", Name = "Cupt Black & White Mug", Price= 17, PictureUri = "https://fakeimg.pl/370x240/EEEEEE/000/?text=CuptBlack&WhiteMug" },
                new CatalogItem() {  Description = "Prism White T-Shirt", Name = "Prism White T-Shirt", Price = 12, PictureUri = "http://fakeimg.pl/370x240/EEEEEE/000/?text=.PrismWhiteT-Shirt" },
                new CatalogItem() {  Description = ".NET Bot Black Sweatshirt", Name = ".NET Bot Black Sweatshirt", Price = 19.5M, PictureUri = "http://fakeimg.pl/370x240/EEEEEE/000/?text=.NETBotBlack" },
                new CatalogItem() {  Description = "Roslyn Red T-Shirt", Name = "Roslyn Red T-Shirt", Price = 12, PictureUri = "http://fakeimg.pl/370x240/EEEEEE/000/?text=RoslynRedT-Shirt"  },
                new CatalogItem() {  Description = "Cupt Black & White Mug", Name = "Cupt Black & White Mug", Price= 17, PictureUri = "https://fakeimg.pl/370x240/EEEEEE/000/?text=CuptBlack&WhiteMug" },
                new CatalogItem() {  Description = "Prism White T-Shirt", Name = "Prism White T-Shirt", Price = 12, PictureUri = "http://fakeimg.pl/370x240/EEEEEE/000/?text=.PrismWhiteT-Shirt" },
                new CatalogItem() {  Description = ".NET Bot Black Sweatshirt", Name = ".NET Bot Black Sweatshirt", Price = 19.5M, PictureUri = "http://fakeimg.pl/370x240/EEEEEE/000/?text=.NETBotBlack" },
                new CatalogItem() {  Description = "Roslyn Red T-Shirt", Name = "Roslyn Red T-Shirt", Price = 12, PictureUri = "http://fakeimg.pl/370x240/EEEEEE/000/?text=RoslynRedT-Shirt"  },
                new CatalogItem() {  Description = "Cupt Black & White Mug", Name = "Cupt Black & White Mug", Price= 17, PictureUri = "https://fakeimg.pl/370x240/EEEEEE/000/?text=CuptBlack&WhiteMug" },
                new CatalogItem() {  Description = "Prism White T-Shirt", Name = "Prism White T-Shirt", Price = 12, PictureUri = "http://fakeimg.pl/370x240/EEEEEE/000/?text=.PrismWhiteT-Shirt" },
                new CatalogItem() {  Description = ".NET Bot Black Sweatshirt", Name = ".NET Bot Black Sweatshirt", Price = 19.5M, PictureUri = "http://fakeimg.pl/370x240/EEEEEE/000/?text=.NETBotBlack" },
                new CatalogItem() {  Description = "Roslyn Red T-Shirt", Name = "Roslyn Red T-Shirt", Price = 12, PictureUri = "http://fakeimg.pl/370x240/EEEEEE/000/?text=RoslynRedT-Shirt"  },
                new CatalogItem() {  Description = "Cupt Black & White Mug", Name = "Cupt Black & White Mug", Price= 17, PictureUri = "https://fakeimg.pl/370x240/EEEEEE/000/?text=CuptBlack&WhiteMug" },
                new CatalogItem() {  Description = "Prism White T-Shirt", Name = "Prism White T-Shirt", Price = 12, PictureUri = "http://fakeimg.pl/370x240/EEEEEE/000/?text=.PrismWhiteT-Shirt" },
                new CatalogItem() {  Description = ".NET Bot Black Sweatshirt", Name = ".NET Bot Black Sweatshirt", Price = 19.5M, PictureUri = "http://fakeimg.pl/370x240/EEEEEE/000/?text=.NETBotBlack" },
                new CatalogItem() {  Description = "Roslyn Red T-Shirt", Name = "Roslyn Red T-Shirt", Price = 12, PictureUri = "http://fakeimg.pl/370x240/EEEEEE/000/?text=RoslynRedT-Shirt"  },
                new CatalogItem() {  Description = "Cupt Black & White Mug", Name = "Cupt Black & White Mug", Price= 17, PictureUri = "https://fakeimg.pl/370x240/EEEEEE/000/?text=CuptBlack&WhiteMug" },
                new CatalogItem() {  Description = "Prism White T-Shirt", Name = "Prism White T-Shirt", Price = 12, PictureUri = "http://fakeimg.pl/370x240/EEEEEE/000/?text=.PrismWhiteT-Shirt" },
                new CatalogItem() {  Description = ".NET Bot Black Sweatshirt", Name = ".NET Bot Black Sweatshirt", Price = 19.5M, PictureUri = "http://fakeimg.pl/370x240/EEEEEE/000/?text=.NETBotBlack" }

            };
        }
    }
}
