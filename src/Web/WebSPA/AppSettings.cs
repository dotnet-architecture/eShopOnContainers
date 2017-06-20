using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eShopOnContainers.WebSPA
{
    public class AppSettings
    {
        public string BaseUrl { get; set; }
        public string CatalogUrl { get; set; }
        public string OrderingUrl { get; set; }
        public string IdentityUrl { get; set; }
        public string BasketUrl { get; set; }
        public bool UseCustomizationData { get; set; }
    }
}
