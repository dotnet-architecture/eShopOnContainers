using Microsoft.eShopOnContainers.Services.Marketing.API.Infrastructure.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.eShopOnContainers.Services.Marketing.API.Dto
{
    public class RuleDTO
    {
        public int Id { get; set; }

        public int RuleTypeId { get; set; }

        public int CampaignId { get; set; }

        public int? LocationId { get; set; }

        public string Description { get; set; }
    }
}