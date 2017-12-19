namespace WebMVC.ViewModels
{
    using System.Collections.Generic;
    using Microsoft.eShopOnContainers.WebMVC.ViewModels;
    using Microsoft.eShopOnContainers.WebMVC.ViewModels.Pagination;
    using WebMVC.ViewModels.Annotations;
    using Newtonsoft.Json;
    using System.ComponentModel.DataAnnotations;

    public class CampaignViewModel
    {
        public IEnumerable<CampaignItem> CampaignItems { get; set; }
        public PaginationInfo PaginationInfo { get; set; }

        [LongitudeCoordinate, Required]
        public double Lon { get; set; } = -122.315752;
        [LatitudeCoordinate, Required]
        public double Lat { get; set; } = 47.604610;
    }
}