using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using eShopWeb.Models.CatalogViewModels;
using eShopWeb.Models;
using eShopWeb.Models.Pagination;
using Microsoft.AspNetCore.Mvc.Rendering;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace eShopWeb.Controllers
{
    public class CatalogController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index(int? BrandFilterApplied, int? TypesFilterApplied, int? page)
        {
            var itemsPage = 10;
            //var catalog = await _catalogSvc.GetCatalogItems(page ?? 0, itemsPage, BrandFilterApplied, TypesFilterApplied);

            var catalog = new List<CatalogItem>();
            var vm = new IndexViewModel()
            {
                CatalogItems = GetPreconfiguredItems(),
                Brands = GetPreconfiguredCatalogBrands(),
                Types = GetPreconfiguredCatalogTypes(),
                BrandFilterApplied = BrandFilterApplied ?? 0,
                TypesFilterApplied = TypesFilterApplied ?? 0,
                PaginationInfo = new PaginationInfo()
                {
                    ActualPage = page ?? 0,
                    ItemsPerPage = (catalog.Count < itemsPage) ? catalog.Count : itemsPage,
                    TotalItems = catalog.Count,
                    TotalPages = int.Parse(Math.Ceiling(((decimal)catalog.Count / itemsPage)).ToString())
                }
            };

            vm.PaginationInfo.Next = (vm.PaginationInfo.ActualPage == vm.PaginationInfo.TotalPages - 1) ? "is-disabled" : "";
            vm.PaginationInfo.Previous = (vm.PaginationInfo.ActualPage == 0) ? "is-disabled" : "";

            return View(vm);
        }


        static IEnumerable<SelectListItem> GetPreconfiguredCatalogBrands()
        {
            return new List<SelectListItem>()
            {
                new SelectListItem() { Value = null, Text="All", Selected= true},
                new SelectListItem() { Value = null, Text = "Azure", Selected= true},
                new SelectListItem() { Value = null, Text = ".NET", Selected= true },
                new SelectListItem() { Value = null, Text = "Visual Studio", Selected= true },
                new SelectListItem() { Value = null, Text = "SQL Server", Selected= true },
                new SelectListItem() { Value = null, Text = "Other", Selected= true }
            };
        }

        static IEnumerable<SelectListItem> GetPreconfiguredCatalogTypes()
        {
            return new List<SelectListItem>()
            {
                new SelectListItem() { Value = null, Text="All", Selected= true},
                new SelectListItem() { Value = null, Text = "Mug", Selected= true },
                new SelectListItem() { Value = null, Text = "T-Shirt", Selected= true },
                new SelectListItem() { Value = null, Text = "Sheet", Selected= true },
                new SelectListItem() { Value = null, Text = "USB Memory Stick", Selected= true }
            };
        }

        static IEnumerable<CatalogItem> GetPreconfiguredItems()
        {
            return new List<CatalogItem>()
            {
                new CatalogItem() { CatalogTypeId=2,CatalogBrandId=2, Description = ".NET Bot Black Sweatshirt", Name = ".NET Bot Black Sweatshirt", Price = 19.5M, PictureUri = "http://externalcatalogbaseurltobereplaced/api/v1/pic/1" },
                new CatalogItem() { CatalogTypeId=1,CatalogBrandId=2, Description = ".NET Black & White Mug", Name = ".NET Black & White Mug", Price= 8.50M, PictureUri = "http://externalcatalogbaseurltobereplaced/api/v1/pic/2" },
                new CatalogItem() { CatalogTypeId=2,CatalogBrandId=5, Description = "Prism White T-Shirt", Name = "Prism White T-Shirt", Price = 12, PictureUri = "http://externalcatalogbaseurltobereplaced/api/v1/pic/3" },
                new CatalogItem() { CatalogTypeId=2,CatalogBrandId=2, Description = ".NET Foundation Sweatshirt", Name = ".NET Foundation Sweatshirt", Price = 12, PictureUri = "http://externalcatalogbaseurltobereplaced/api/v1/pic/4" },
                new CatalogItem() { CatalogTypeId=3,CatalogBrandId=5, Description = "Roslyn Red Sheet", Name = "Roslyn Red Sheet", Price = 8.5M, PictureUri = "http://externalcatalogbaseurltobereplaced/api/v1/pic/5" },
                new CatalogItem() { CatalogTypeId=2,CatalogBrandId=2, Description = ".NET Blue Sweatshirt", Name = ".NET Blue Sweatshirt", Price = 12, PictureUri = "http://externalcatalogbaseurltobereplaced/api/v1/pic/6" },
                new CatalogItem() { CatalogTypeId=2,CatalogBrandId=5, Description = "Roslyn Red T-Shirt", Name = "Roslyn Red T-Shirt", Price = 12, PictureUri = "http://externalcatalogbaseurltobereplaced/api/v1/pic/7"  },
                new CatalogItem() { CatalogTypeId=2,CatalogBrandId=5, Description = "Kudu Purple Sweatshirt", Name = "Kudu Purple Sweatshirt", Price = 8.5M, PictureUri = "http://externalcatalogbaseurltobereplaced/api/v1/pic/8" },
                new CatalogItem() { CatalogTypeId=1,CatalogBrandId=5, Description = "Cup<T> White Mug", Name = "Cup<T> White Mug", Price = 12, PictureUri = "http://externalcatalogbaseurltobereplaced/api/v1/pic/9" },
                new CatalogItem() { CatalogTypeId=3,CatalogBrandId=2, Description = ".NET Foundation Sheet", Name = ".NET Foundation Sheet", Price = 12, PictureUri = "http://externalcatalogbaseurltobereplaced/api/v1/pic/10" },
                new CatalogItem() { CatalogTypeId=3,CatalogBrandId=2, Description = "Cup<T> Sheet", Name = "Cup<T> Sheet", Price = 8.5M, PictureUri = "http://externalcatalogbaseurltobereplaced/api/v1/pic/11" },
                new CatalogItem() { CatalogTypeId=2,CatalogBrandId=5, Description = "Prism White TShirt", Name = "Prism White TShirt", Price = 12, PictureUri = "http://externalcatalogbaseurltobereplaced/api/v1/pic/12" }
            };
        }
    }
}
