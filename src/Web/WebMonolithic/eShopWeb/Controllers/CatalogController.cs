using Microsoft.eShopWeb.Services;
using Microsoft.eShopWeb.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Microsoft.eShopWeb.Controllers
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

            var vm = new CatalogIndex()
            {
                CatalogItems = catalog.Data,
                Brands = await _catalogSvc.GetBrands(),
                Types = await _catalogSvc.GetTypes(),
                BrandFilterApplied = BrandFilterApplied ?? 0,
                TypesFilterApplied = TypesFilterApplied ?? 0,
                PaginationInfo = new PaginationInfo()
                {
                    ActualPage = page ?? 0,
                    ItemsPerPage = catalog.Data.Count,
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
        
        public IActionResult Error()
        {
            return View();
        }
    }
}
