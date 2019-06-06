using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eShopOnContainers.WebSPA
{
    public class AppSettings
    {
        public string IdentityUrl { get; set; }
        public string BasketUrl { get; set; }
        public string MarketingUrl { get; set; }

        public string PurchaseUrl { get; set; }
        public string SignalrHubUrl { get; set; }

        public string ActivateCampaignDetailFunction { get; set; }
        public bool UseCustomizationData { get; set; }
    }
}
