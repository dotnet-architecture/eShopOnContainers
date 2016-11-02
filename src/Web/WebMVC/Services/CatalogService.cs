using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.eShopOnContainers.WebMVC.Models;
using Microsoft.CodeAnalysis.Options;
using Microsoft.Extensions.Options;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Microsoft.eShopOnContainers.WebMVC.Services
{
    public class CatalogService : ICatalogService
    {
        private readonly List<CatalogItem> _items;          //Fake data while services are ready. 
        private readonly IOptions<AppSettings> _settings;
        private HttpClient _apiClient;
        private readonly string _remoteServiceBaseUrl;
        private int _totalItems;

        public int TotalItems 
        {
            get
            {
                return _totalItems;
            }
        }

        public CatalogService(IOptions<AppSettings> settings) {
            _settings = settings;

            #region fake data
            _items = new List<CatalogItem>()
            {
                new CatalogItem() { Id = Guid.NewGuid().ToString(), Description = "Roslyn Red T-Shirt", Name = "Roslyn Red T-Shirt", Price = 12, PictureUrl = "https://fakeimg.pl/370x240/EEEEEE/000/?text=RoslynRedT-Shirt"  },
                new CatalogItem() { Id = Guid.NewGuid().ToString(), Description = "Cupt Black & White Mug", Name = "Cupt Black & White Mug", Price= 17, PictureUrl = "https://fakeimg.pl/370x240/EEEEEE/000/?text=CuptBlack&WhiteMug" },
                new CatalogItem() { Id = Guid.NewGuid().ToString(), Description = "Prism White T-Shirt", Name = "Prism White T-Shirt", Price = 12, PictureUrl = "http://fakeimg.pl/370x240/EEEEEE/000/?text=.PrismWhiteT-Shirt" },
                new CatalogItem() { Id = Guid.NewGuid().ToString(), Description = ".NET Bot Black Sweatshirt", Name = ".NET Bot Black Sweatshirt", Price = decimal.Parse("19.5"), PictureUrl = "http://fakeimg.pl/370x240/EEEEEE/000/?text=.NETBotBlack" },
                new CatalogItem() { Id = Guid.NewGuid().ToString(), Description = "Roslyn Red T-Shirt", Name = "Roslyn Red T-Shirt", Price = 12, PictureUrl = "http://fakeimg.pl/370x240/EEEEEE/000/?text=RoslynRedT-Shirt"  },
                new CatalogItem() { Id = Guid.NewGuid().ToString(), Description = "Cupt Black & White Mug", Name = "Cupt Black & White Mug", Price= 17, PictureUrl = "https://fakeimg.pl/370x240/EEEEEE/000/?text=CuptBlack&WhiteMug" },
                new CatalogItem() { Id = Guid.NewGuid().ToString(), Description = "Prism White T-Shirt", Name = "Prism White T-Shirt", Price = 12, PictureUrl = "http://fakeimg.pl/370x240/EEEEEE/000/?text=.PrismWhiteT-Shirt" },
                new CatalogItem() { Id = Guid.NewGuid().ToString(), Description = ".NET Bot Black Sweatshirt", Name = ".NET Bot Black Sweatshirt", Price = decimal.Parse("19.5"), PictureUrl = "http://fakeimg.pl/370x240/EEEEEE/000/?text=.NETBotBlack" },
                new CatalogItem() { Id = Guid.NewGuid().ToString(), Description = "Roslyn Red T-Shirt", Name = "Roslyn Red T-Shirt", Price = 12, PictureUrl = "http://fakeimg.pl/370x240/EEEEEE/000/?text=RoslynRedT-Shirt"  },
                new CatalogItem() { Id = Guid.NewGuid().ToString(), Description = "Cupt Black & White Mug", Name = "Cupt Black & White Mug", Price= 17, PictureUrl = "https://fakeimg.pl/370x240/EEEEEE/000/?text=CuptBlack&WhiteMug" },
                new CatalogItem() { Id = Guid.NewGuid().ToString(), Description = "Prism White T-Shirt", Name = "Prism White T-Shirt", Price = 12, PictureUrl = "http://fakeimg.pl/370x240/EEEEEE/000/?text=.PrismWhiteT-Shirt" },
                new CatalogItem() { Id = Guid.NewGuid().ToString(), Description = ".NET Bot Black Sweatshirt", Name = ".NET Bot Black Sweatshirt", Price = decimal.Parse("19.5"), PictureUrl = "http://fakeimg.pl/370x240/EEEEEE/000/?text=.NETBotBlack" },
                new CatalogItem() { Id = Guid.NewGuid().ToString(), Description = "Roslyn Red T-Shirt", Name = "Roslyn Red T-Shirt", Price = 12, PictureUrl = "http://fakeimg.pl/370x240/EEEEEE/000/?text=RoslynRedT-Shirt"  },
                new CatalogItem() { Id = Guid.NewGuid().ToString(), Description = "Cupt Black & White Mug", Name = "Cupt Black & White Mug", Price= 17, PictureUrl = "https://fakeimg.pl/370x240/EEEEEE/000/?text=CuptBlack&WhiteMug" },
                new CatalogItem() { Id = Guid.NewGuid().ToString(), Description = "Prism White T-Shirt", Name = "Prism White T-Shirt", Price = 12, PictureUrl = "http://fakeimg.pl/370x240/EEEEEE/000/?text=.PrismWhiteT-Shirt" },
                new CatalogItem() { Id = Guid.NewGuid().ToString(), Description = ".NET Bot Black Sweatshirt", Name = ".NET Bot Black Sweatshirt", Price = decimal.Parse("19.5"), PictureUrl = "http://fakeimg.pl/370x240/EEEEEE/000/?text=.NETBotBlack" },
                new CatalogItem() { Id = Guid.NewGuid().ToString(), Description = "Roslyn Red T-Shirt", Name = "Roslyn Red T-Shirt", Price = 12, PictureUrl = "http://fakeimg.pl/370x240/EEEEEE/000/?text=RoslynRedT-Shirt"  },
                new CatalogItem() { Id = Guid.NewGuid().ToString(), Description = "Cupt Black & White Mug", Name = "Cupt Black & White Mug", Price= 17, PictureUrl = "https://fakeimg.pl/370x240/EEEEEE/000/?text=CuptBlack&WhiteMug" },
                new CatalogItem() { Id = Guid.NewGuid().ToString(), Description = "Prism White T-Shirt", Name = "Prism White T-Shirt", Price = 12, PictureUrl = "http://fakeimg.pl/370x240/EEEEEE/000/?text=.PrismWhiteT-Shirt" },
                new CatalogItem() { Id = Guid.NewGuid().ToString(), Description = ".NET Bot Black Sweatshirt", Name = ".NET Bot Black Sweatshirt", Price = decimal.Parse("19.5"), PictureUrl = "http://fakeimg.pl/370x240/EEEEEE/000/?text=.NETBotBlack" },
                new CatalogItem() { Id = Guid.NewGuid().ToString(), Description = "Roslyn Red T-Shirt", Name = "Roslyn Red T-Shirt", Price = 12, PictureUrl = "http://fakeimg.pl/370x240/EEEEEE/000/?text=RoslynRedT-Shirt"  },
                new CatalogItem() { Id = Guid.NewGuid().ToString(), Description = "Cupt Black & White Mug", Name = "Cupt Black & White Mug", Price= 17, PictureUrl = "https://fakeimg.pl/370x240/EEEEEE/000/?text=CuptBlack&WhiteMug" },
                new CatalogItem() { Id = Guid.NewGuid().ToString(), Description = "Prism White T-Shirt", Name = "Prism White T-Shirt", Price = 12, PictureUrl = "http://fakeimg.pl/370x240/EEEEEE/000/?text=.PrismWhiteT-Shirt" },
                new CatalogItem() { Id = Guid.NewGuid().ToString(), Description = ".NET Bot Black Sweatshirt", Name = ".NET Bot Black Sweatshirt", Price = decimal.Parse("19.5"), PictureUrl = "http://fakeimg.pl/370x240/EEEEEE/000/?text=.NETBotBlack" },
                new CatalogItem() { Id = Guid.NewGuid().ToString(), Description = "Roslyn Red T-Shirt", Name = "Roslyn Red T-Shirt", Price = 12, PictureUrl = "http://fakeimg.pl/370x240/EEEEEE/000/?text=RoslynRedT-Shirt"  },
                new CatalogItem() { Id = Guid.NewGuid().ToString(), Description = "Cupt Black & White Mug", Name = "Cupt Black & White Mug", Price= 17, PictureUrl = "https://fakeimg.pl/370x240/EEEEEE/000/?text=CuptBlack&WhiteMug" },
                new CatalogItem() { Id = Guid.NewGuid().ToString(), Description = "Prism White T-Shirt", Name = "Prism White T-Shirt", Price = 12, PictureUrl = "http://fakeimg.pl/370x240/EEEEEE/000/?text=.PrismWhiteT-Shirt" },
                new CatalogItem() { Id = Guid.NewGuid().ToString(), Description = ".NET Bot Black Sweatshirt", Name = ".NET Bot Black Sweatshirt", Price = decimal.Parse("19.5"), PictureUrl = "http://fakeimg.pl/370x240/EEEEEE/000/?text=.NETBotBlack" }
            };
            #endregion
        }
         
        public CatalogItem GetCatalogItem(string Id)
        {
            return _items.Where(x => x.Id.Equals(Id)).FirstOrDefault();
        }

        public Task<List<CatalogItem>> GetCatalogItems(int? skip,int? take)
        {
            var res = _items;

            _totalItems = _items.Count();

            if (skip.HasValue)
                return Task.Run(() => { return _items.Skip(skip.Value).Take(take.Value).ToList(); });
            else
                return Task.Run(() => { return _items; });
        }

        public IEnumerable<SelectListItem> GetBrands()
        {
            var items = new List<SelectListItem>();
            items.Add(new SelectListItem() { Value = "0", Text = "All", Selected = true });
            items.Add(new SelectListItem() { Value = "1", Text = "Visual Studio" });
            items.Add(new SelectListItem() { Value = "2", Text = "Azure" });

            return items;
        }

        public IEnumerable<SelectListItem> GetTypes()
        {
            var items = new List<SelectListItem>();
            items.Add(new SelectListItem() { Value = "0", Text = "All", Selected = true });
            items.Add(new SelectListItem() { Value = "1", Text = "Mug" });
            items.Add(new SelectListItem() { Value = "2", Text = "T-Shirt" });
            

            return items;
        }
    }
}
