namespace Microsoft.eShopOnContainers.Services.Marketing.API.Model
{
    using System;
    using System.Collections.Generic;

    public class Campaign
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public DateTime From { get; set; }

        public DateTime To { get; set; }

        public string Url { get; set; }

        public ICollection<Rule> Rules { get; set; }
    }
}