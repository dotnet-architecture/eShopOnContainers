namespace WebMVC.ViewModels
{
    using System.Collections.Generic;
    using Microsoft.eShopOnContainers.WebMVC.ViewModels;
    using Microsoft.eShopOnContainers.WebMVC.ViewModels.Pagination;

    public class CampaignViewModel
    {
        public IEnumerable<CampaignItem> CampaignItems { get; set; }
        public PaginationInfo PaginationInfo { get; set; }
    }
}