namespace Microsoft.eShopOnContainers.Services.Marketing.API.Model
{
    using System;
    using System.Collections.Generic;

    public class Campaign
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime From { get; set; }

        public DateTime To { get; set; }

        public string PictureName { get; set; }

        public string PictureUri { get; set; }

        public string DetailsUri { get; set; }

        public List<Rule> Rules { get; set; }


        public Campaign()
        {
            Rules = new List<Rule>();
        }
    }
}