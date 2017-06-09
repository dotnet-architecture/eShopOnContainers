namespace Microsoft.eShopOnContainers.Services.Locations.API.Model
{
    using MongoDB.Bson.Serialization.Attributes;
    using MongoDB.Bson;
    using System;
    using System.Collections.Generic;

    public class UserLocation
    {
        [BsonIgnoreIfDefault]
        public ObjectId Id { get; set; }
        public int UserId { get; set; } = 0;
        public ObjectId LocationId { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
