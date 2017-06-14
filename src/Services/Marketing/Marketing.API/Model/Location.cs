using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.eShopOnContainers.Services.Marketing.API.Model
{
    public class Location
    {
        public int LocationId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
    }
}
