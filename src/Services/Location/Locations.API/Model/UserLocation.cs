namespace Microsoft.eShopOnContainers.Services.Locations.API.Model
{
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;
    using System;

    public class UserLocation
    {
        [BsonIgnoreIfDefault]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public int UserId { get; set; } = 0;
        [BsonRepresentation(BsonType.ObjectId)]
        public string LocationId { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
