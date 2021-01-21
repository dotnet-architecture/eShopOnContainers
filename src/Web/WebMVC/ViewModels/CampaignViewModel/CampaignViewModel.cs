namespace WebMVC.ViewModels
{
    using Microsoft.eShopOnContainers.WebMVC.ViewModels;
    using Microsoft.eShopOnContainers.WebMVC.ViewModels.Pagination;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using WebMVC.ViewModels.Annotations;

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