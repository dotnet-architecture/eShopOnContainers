using System;

namespace Microsoft.eShopOnContainers.WebMVC.ViewModels
{
    public record CampaignItem
    {
        public int Id { get; init; }

        public string Name { get; init; }

        public string Description { get; init; }

        public DateTime From { get; init; }

        public DateTime To { get; init; }

        public string PictureUri { get; init; }
        public string DetailsUri { get; init; }
    }
}