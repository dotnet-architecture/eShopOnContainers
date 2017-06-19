namespace Microsoft.eShopOnContainers.WebMVC.ViewModels
{
    using System.Collections.Generic;

    public class Campaign
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int Count { get; set; }
        public List<CampaignItem> Data { get; set; }
    }
}