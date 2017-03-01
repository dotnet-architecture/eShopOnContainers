using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using eShopWeb.Models.CatalogViewModels;
using eShopWeb.Models;
using eShopWeb.Models.Pagination;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using eShopWeb.Services;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace eShopWeb.Controllers
{
    public class CatalogController : Controller
    {
        private readonly IHostingEnvironment _env;
        private readonly ICatalogService _catalogSvc;

        public CatalogController(IHostingEnvironment env, ICatalogService catalogSvc)
        {
            _env = env;
            _catalogSvc = catalogSvc;
        }   


        // GET: /<controller>/
        public async Task<IActionResult> Index(int? BrandFilterApplied, int? TypesFilterApplied, int? page)
        {
            var itemsPage = 10;           
            var catalog = await _catalogSvc.GetCatalogItems(page ?? 0, itemsPage, BrandFilterApplied, TypesFilterApplied);

            var vm = new IndexViewModel()
            {
                CatalogItems = catalog.Data,
                Brands = await _catalogSvc.GetBrands(),
                Types = await _catalogSvc.GetTypes(),
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

        [HttpGet("{id}")]
        [Route("[controller]/pic/{id}")]
        // GET: /<controller>/pic/{id}
        public IActionResult GetImage(int id)
        {
            var contentRoot = _env.ContentRootPath + "//Pics";
            var path = Path.Combine(contentRoot, id + ".png");
            Byte[] b = System.IO.File.ReadAllBytes(path);
            return File(b, "image/png");

        }        
    }
}
